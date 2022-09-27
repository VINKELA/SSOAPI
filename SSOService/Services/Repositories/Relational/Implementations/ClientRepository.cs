using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SSOService.Extensions;
using SSOService.Models;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Models.Domains;
using SSOService.Models.DTOs;
using SSOService.Models.DTOs.Application;
using SSOService.Models.DTOs.Client;
using SSOService.Models.DTOs.Permission;
using SSOService.Models.DTOs.Role;
using SSOService.Models.DTOs.Service;
using SSOService.Models.DTOs.ReSourceType;
using SSOService.Models.DTOs.User;
using SSOService.Models.Enums;
using SSOService.Services.General.Interfaces;
using SSOService.Services.Interfaces;
using SSOService.Services.Repositories.NonRelational.Interfaces;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Implementations
{
    public class ClientRepository : IClientRepository
    {
        private const string Email = "Email";
        private const string Name = "Name";
        private const string EntityName = "client";
        private const string Type = "Client Type";
        private readonly SSODbContext _db;
        private readonly IServiceResponse _response;
        private readonly GetClientDTO ReturnType = null;
        private readonly GetClientSubscription ClientSubscriptionReturnType = new();
        private readonly IFileRepository _fileRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUserRepository _userReoository;
        private readonly IConfiguration _configuration;
        private readonly IRoleRepository _roleRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IResourceType _resourceType;
        private readonly IResourceRepository _resourceRepository;
        private readonly IPermissionRepository _permissionRepository;
        public ClientRepository(SSODbContext db, IServiceResponse response, IFileRepository fileRepository, IApplicationRepository applicationRepository,
            IResourceType resourceType, IConfiguration configuration, ISubscriptionRepository subscriptionRepository, IUserRepository userRepository,
            IRoleRepository roleRepository, IResourceRepository resourceRepository,IPermissionRepository permissionRepository)
        {
            _db = db;
            _response = response;
            _fileRepository = fileRepository;
            _subscriptionRepository = subscriptionRepository;
            _userReoository = userRepository;
            _roleRepository = roleRepository;
            _applicationRepository = applicationRepository;
            _resourceRepository = resourceRepository;
            _resourceType = resourceType;
            _configuration = configuration;
            _permissionRepository = permissionRepository;
        }
        public async Task<Response<GetClientDTO>> Save(CreateClientDTO client)
        {
            var operation = await CreateClient(client);
            if (operation.Status)
                await CreateAdminUser(client);
            return operation;
        }
        public async Task<Response<GetClientDTO>> InitializeApp(CreateClientDTO client)
        => await InitializeApplication(client);
            
        public async Task<Response<GetClientDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false)
        {

            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, EntityName));
            if (deactivate) current.IsActive = !deactivate;
            else if (delete)
            {
                current.IsActive = !delete;
                current.IsDeleted = delete;
            }
            else current.IsActive = true;
            var hasChanged = await HasChanged(current);
            if (hasChanged)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.EntityChangedByAnotherUser,
                    current.Name));
            current.ConcurrencyStamp = Guid.NewGuid();
            _db.Clients.Update(current);
            var result = await _db.SaveChangesAsync();
            return result > 0 ? _response.SuccessResponse(Todto(current)) :
            _response.FailedResponse(ReturnType);
        }
        public async Task<Response<List<GetClientDTO>>> Get(string name, string contactPersonEmail,
            ClientType? clientType)
        {
            var returnType = new List<GetClientDTO>();
            var list = await _db.Clients.Where(x => !x.IsDeleted).ToListAsync();
            if (!string.IsNullOrEmpty(name))
                list = list.Where(x => x.Name.Contains(name.Trim().ToUpper())).ToList();
            if (!string.IsNullOrEmpty(contactPersonEmail))
                list = list.Where(x => x.ContactPersonEmail.Contains(contactPersonEmail.Trim().ToUpper())).ToList();
            if (clientType != null)
                list = list.Where(x => x.ClientType == clientType).ToList();
            returnType.AddRange(list.Select(x => Todto(x)));
            return _response.SuccessResponse(returnType);
        }
        public async Task<Response<GetClientDTO>> Get(Guid id)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, EntityName));
            return _response.SuccessResponse(Todto(current));
        }
        public async Task<GetClientDTO> Get(string code)
        {
            var current = await _db.Clients.FirstOrDefaultAsync(x => x.Code == code);
            if (current == null)
                return null;
            return Todto(current);
        }
        public async Task<Response<GetClientSubscription>> AddSubscription(Guid subscriptionId, Guid clientId)
        {


            var subscription = await _subscriptionRepository.GetSubscriptionById(subscriptionId);
            var client = await Exists(clientId);

            if (subscription == null)
                return _response.FailedResponse(ClientSubscriptionReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Subscription));
            if (client == null)
                return _response.FailedResponse(ClientSubscriptionReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Client));
            var newAuth = new ClientSubscription
            {
                SubscriptionId = subscriptionId,
                ClientId = clientId
            };
            await _db.AddAsync(newAuth);
            var status = await _db.SaveChangesAsync() > 0;
            if (status) return _response.SuccessResponse(await ToDto(newAuth.Code));
            return _response.FailedResponse(ClientSubscriptionReturnType);
        }
        public async Task<Response<GetClientSubscription>> UpdateClientSubscription(Guid subscriptionId, Guid clientId, bool update)
        {

            var current = await _db.ClientSubscriptions.FirstOrDefaultAsync(x => x.SubscriptionId == subscriptionId && x.ClientId == clientId);
            if (current == null)
                return _response.FailedResponse(ClientSubscriptionReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.Client));
            current.IsActive = update ? !current.IsActive : current.IsActive;
            _db.Update(current);
            var status = await _db.SaveChangesAsync() > 0;
            if (status) return _response.SuccessResponse(await ToDto(current.Code));
            return _response.FailedResponse(ClientSubscriptionReturnType);
        }
        private async Task<Response<GetClientDTO>> InitializeApplication(CreateClientDTO createClient)
        {
            if (!_db.Clients.Any())
            {
                var appName = _configuration[SetUpConstants.AppName];
                var appBaseUrl = _configuration[SetUpConstants.ApplicationBaseUrl];
                var resourceType = _configuration[SetUpConstants.ResourceType];
                var clientDetails = await CreateClient(createClient);
                // create sso  application
                var applicationDTO = new CreateApplicationDTO
                {
                    Name = appName,
                    ApplicationType = ApplicationType.Service,
                    ClientId = clientDetails.Data.Id,
                    URL = appBaseUrl
                };
                var application = await _applicationRepository.Create(applicationDTO);
                var resourceTypeDTO = new CreateResourceTypeDTO
                {
                    Name = resourceType,
                    ApplicationId = application.Data.Id,
                };
                var createdResourceType = await _resourceType.Create(resourceTypeDTO);
                var type = typeof(DefaultResources);
                var defaultResources = GetConstants(type);
                var permissions = new List<CreatePermissionDTO>();
                var resources = new List<CreateResourceDTO>();
                foreach (var resource in defaultResources)
                {
                    resources.Add(new CreateResourceDTO
                    {
                        Name = resource.GetRawConstantValue()?.ToString(),
                        ResourceTypeId = createdResourceType.Data.Id,
                        ApplicationId = application.Data.Id
                    });
                }
                // create resources
                var createdResources = await _resourceRepository.Create(resources);
                foreach (var resource in defaultResources)
                {
                    var resourceId = createdResources.Data.
                        FirstOrDefault(y => y.Name == resource
                        .GetRawConstantValue()?.ToString()).Id;

                    permissions.Add(new CreatePermissionDTO
                    {
                        Name = $"{resource.GetRawConstantValue()?.ToString()} {DefaultResources.Permission}",
                        PermissionType = PermissionType.All,
                        Scope = Scope.all,
                        ResourceId = resourceId,
                    });
                    permissions.Add(new CreatePermissionDTO
                    {
                        Name = $"{resource.GetRawConstantValue()?.ToString()} {DefaultResources.Permission}",
                        PermissionType = PermissionType.All,
                        Scope = Scope.clientResource,
                        ResourceId = resourceId
                    });
                }


                await _permissionRepository.Create(permissions);
                // create super admin User
                await CreateAdminUser(createClient, true);
                return clientDetails;
            }
            return _response.FailedResponse<GetClientDTO>(null);
        }
        public async Task<Response<GetClientDTO>> Update(Guid id, UpdateClientDTO client)
        {

            var returnType = new GetClientDTO();
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(returnType, string.Format(ValidationConstants.FieldNotFound, EntityName));
            if (!string.IsNullOrEmpty(client.ContactPersonEmail) && !client.ContactPersonEmail.IsValidEmailFormat())
                return _response.FailedResponse(returnType, string.Format(ValidationConstants.InvalidFieldFormatResponse, Email));
            if (client.ClientType == ClientType.Unknown || !Enum.GetValues<ClientType>().Contains(client.ClientType))
                return _response.FailedResponse(returnType,
                    string.Format(ValidationConstants.InvalidFieldResponse, client.ClientType, Type));
            Guid parentId = Guid.Empty;
            if (!string.IsNullOrEmpty(client.ParentClientId))
            {
                _ = Guid.TryParse(client.ParentClientId, out parentId);
                var parent = Exists(parentId);
                if (parent == null) return
                         _response.FailedResponse(returnType, string.Format(ValidationConstants.FieldNotFound, EntityName));
            }
            string filePath = null;
            if (client.Image != null)
            {
                filePath = await _fileRepository.Save(client.Image, client.Name, FileType.UserImage);
            }

            current.Address = string.IsNullOrEmpty(client.Address) ? current.Address : client.Address.Trim().ToLower();
            current.ClientType = client.ClientType != ClientType.Unknown ? client.ClientType : current.ClientType;
            current.ContactPerson = string.IsNullOrEmpty(client.ContactPerson) ?
                current.ContactPerson : client.ContactPerson.Trim().ToUpper();
            current.ContactPersonPhoneNumber = string.IsNullOrEmpty(client.ContactPersonPhoneNumber) ?
                current.ContactPersonPhoneNumber : client.ContactPersonPhoneNumber.Trim();
            current.Country = string.IsNullOrEmpty(client.Country) ? current.Country : client.Country.Trim().ToUpper();
            current.LogoUrl = string.IsNullOrEmpty(filePath) ? current.LogoUrl : filePath;
            current.Motto = string.IsNullOrEmpty(client.Motto) ? current.Motto : client.Motto.Trim().ToLower();
            current.Name = string.IsNullOrEmpty(client.Name) ? current.Name : client.Name.Trim().ToUpper();
            current.ParentClientId = string.IsNullOrEmpty(client.ParentClientId) ? parentId : current.ParentClientId;
            current.Modified = DateTime.Now;
            var hasChanged = await HasChanged(current);
            if (hasChanged)
                return _response.FailedResponse(returnType, string.Format(ValidationConstants.EntityChangedByAnotherUser, client.Name));
            current.ConcurrencyStamp = Guid.NewGuid();
            _db.Clients.Update(current);
            var status = await _db.SaveChangesAsync();
            return status > 0 ? _response.SuccessResponse(Todto(current)) :
            _response.FailedResponse(returnType);
        }
        private async Task<Client> Exists(Guid id)
        {
            var current = await _db.Clients.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            return current;
        }
        private async Task<bool> HasChanged(Client client)
        {
            var lastest = await Exists(client.Id);
            return !(client.ConcurrencyStamp == lastest.ConcurrencyStamp);
        }
        private static List<FieldInfo> GetConstants(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
                 BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        }
        private async Task<GetClientSubscription> ToDto(string code)
        {
            var current = await _db.ClientSubscriptions.FirstOrDefaultAsync(x => x.Code == code);
            var client = await Exists(current.ClientId);
            var subscription = await _subscriptionRepository.Get(current.SubscriptionId);

            return new GetClientSubscription
            {
                Client = client.Name,
                ClientId = client.Id,
                Status = current.IsActive,
                SubcriptionId = subscription.Data.Id,
                Subscription = subscription.Data.Name,
            };
        }
        private GetClientDTO Todto(Client client)
        {
            var parent = _db.Clients.Where(x => x.Id == client.ParentClientId).FirstOrDefault();
            var details = _db.Clients.Where(x => x.Code == client.Code).FirstOrDefault();
            var parentName = parent != null ? parent.Name : ValidationConstants.NotAvailable;

            return new GetClientDTO
            {
                Address = client.Address == null ? ValidationConstants.NotAvailable : client.Address.ToTitleCase(),
                ClientType = client.ClientType,
                ClientTypeName = client.ClientType > ClientType.Unknown ? client.ClientType.DisplayName().ToTitleCase()
                : ValidationConstants.NotAvailable,
                ContactPersonEmail = client.ContactPersonEmail != null ? client.ContactPersonEmail.ToLower() : ValidationConstants.NotAvailable,
                Country = client.Country != null ? client.Country.ToTitleCase() : ValidationConstants.NotAvailable,
                Id = details.Id,
                LogoUrl = client.LogoUrl ?? ValidationConstants.NotAvailable,
                Motto = client.Motto ?? ValidationConstants.NotAvailable,
                Name = client.Name.ToTitleCase(),
                ParentClient = client.ParentClientId != null && client.ParentClientId != Guid.Empty ? client.ParentClientId.ToString() : ValidationConstants.NotAvailable,
                ParentClientName = parentName,
                State = client.State != null ? client.State.ToTitleCase() : ValidationConstants.NotAvailable,
                IsActive = client.IsActive,
                Logo = !string.IsNullOrEmpty(client.LogoUrl) ? _fileRepository.Get(client.LogoUrl, FileType.ClientLogo) : null
            };
        }
        private async Task CreateAdminUser(CreateClientDTO client, bool createAppAdmin = false)
        {
            var clientDetails = await _db.Clients.FirstOrDefaultAsync(x => x.ContactPersonEmail == client.ContactPersonEmail);
            var role = new CreateRoleDTO
            {
                Name = createAppAdmin? RoleConstants.DefaultApplicationAdmin: RoleConstants.DefaultClientAdmin,
                ClientId = clientDetails.Id
            };
            var appName = _configuration[SetUpConstants.AppName];
            var roleDetails = await _roleRepository.Create(role);
            var sso = await _db.Applications.FirstOrDefaultAsync(x => x.Name == appName);
            var resourceType = await _db.ResourceTypes.FirstOrDefaultAsync(x => x.ApplicationId == sso.Id);
            var resources = await _db.Resources.Where(x => x.ResourceTypeId == resourceType.Id).ToListAsync();
            var resourceIds = resources.Select(x => x.Id);
            var permissions = new List<Permission>();
            var scope = createAppAdmin ? Scope.all : Scope.clientResource;
             permissions =  await _db.Permissions.Where(x => resourceIds.Contains(x.ResourceId) 
            && x.Scope == scope).ToListAsync();
            //create a user
            var tempPassword = Guid.NewGuid().ToString()+"A";
            var user = new CreateUserDTO
            {
                FirstName = client.ContactPersonFirstName,
                LastName = client.ContactPersonLastName,
                Email = client.ContactPersonEmail,
                PhoneNumber = client.ContactPersonPhoneNumber,
                Password =tempPassword,
                Confirmation = tempPassword
            };
            var userDetails = await _userReoository.CreateAsync(user);
            await _userReoository.AddRole(roleDetails.Data.Id, userDetails.Data.Id);
            var permissionsIds = permissions.Select(x => x.Id).ToList();
            await _userReoository.AddPermission(permissionsIds, userDetails.Data.Id);
            //send an email
        }
        private async Task<Response<GetClientDTO>> CreateClient(CreateClientDTO client)
        {

            if (string.IsNullOrEmpty(client.Name))
                return _response.FailedResponse(ReturnType,
                string.Format(ValidationConstants.InvalidFieldFormatResponse, Name));
            if (string.IsNullOrEmpty(client.ContactPersonEmail))
                return _response.FailedResponse(ReturnType,
                string.Format(ValidationConstants.InvalidFieldFormatResponse, Email));
            if (!client.ContactPersonEmail.IsValidEmailFormat())
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.InvalidFieldFormatResponse, Email));
            if (client.ClientType == ClientType.Unknown || !Enum.GetValues<ClientType>().Contains(client.ClientType))
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.InvalidFieldResponse, client.ClientType, Type));
            string filePath = null;
            if (client.Logo != null)
            {
                filePath = await _fileRepository.Save(client.Logo, client.Name, FileType.ClientLogo);
            }

            var newClient = new Client()
            {
                ClientType = client.ClientType,
                ContactPersonEmail = client.ContactPersonEmail.Trim().ToUpper(),
                Name = client.Name.Trim().ToUpper(),
                LogoUrl = filePath
            };
            _db.Clients.Add(newClient);
            var status = await _db.SaveChangesAsync() > 0;
            var clientDetails = Todto(newClient);
            return status ? _response.SuccessResponse(clientDetails) : _response.FailedResponse(ReturnType);
        }
    }


}

