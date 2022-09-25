using Microsoft.EntityFrameworkCore;
using SSOService.Extensions;
using SSOService.Models;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Models.Domains;
using SSOService.Models.DTOs;
using SSOService.Models.DTOs.Client;
using SSOService.Models.Enums;
using SSOService.Services.General.Interfaces;
using SSOService.Services.Repositories.NonRelational.Interfaces;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly GetClientDTO ReturnType = new();
        private readonly GetClientSubscription ClientSubscriptionReturnType = new();

        private readonly IFileRepository _fileRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUserRepository _userReoository;



        public ClientRepository(SSODbContext db, IServiceResponse response, IFileRepository fileRepository,
            ISubscriptionRepository subscriptionRepository, IUserRepository userRepository)
        {
            _db = db;
            _response = response;
            _fileRepository = fileRepository;
            _subscriptionRepository = subscriptionRepository;
            _userReoository = userRepository;

        }

        public async Task<Response<GetClientDTO>> Save(CreateClientDTO client)
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
            var status = await _db.SaveChangesAsync();
            var clientDetails = Todto(newClient);
            if (status > 0)
            {
                var role = new CreateRoleDTO
                {
                    Name = "superadmin",
                    ClientId = clientDetails.Data.Id
                };
                var roleDetails = await _roleRepository.Create(role);
                //create a user
                var user = new CreateUserDTO
                {
                    FirstName = client.ContactPersonFirstName,
                    LastName = client.ContactPersonLastName,
                    Email = client.ContactPersonEmail
                };
                var userDetails = await _userReoository.CreateAsync(user);
                await _userReoository.AddRole(roleDetails.Data.Id, userDetails.Data.Id);
                //send an email
                return _response.SuccessResponse(clientDetails);
            }
            return _response.FailedResponse(ReturnType);
        }

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
                return _response.FailedResponse(ClientSubscriptionReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Subscription));
            if (client == null)
                return _response.FailedResponse(ClientSubscriptionReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Client));
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
                return _response.FailedResponse(ClientSubscriptionReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Client));
            current.IsActive = update ? !current.IsActive : current.IsActive;
            _db.Update(current);
            var status = await _db.SaveChangesAsync() > 0;
            if (status) return _response.SuccessResponse(await ToDto(current.Code));
            return _response.FailedResponse(ClientSubscriptionReturnType);
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
                ContactPerson = client.ContactPerson != null ? client.ContactPerson.ToTitleCase() : ValidationConstants.NotAvailable,
                ContactPersonEmail = client.ContactPersonEmail != null ? client.ContactPersonEmail.ToLower() : ValidationConstants.NotAvailable,
                ContactPersonPhoneNumber = client.ContactPersonPhoneNumber ?? ValidationConstants.NotAvailable,
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
        public async Task InitializeApplication(){
           if(!_db.Users.Any()){
                var details = _Configuration.GetSection("ApplicationDetails");
                var clientName = details["ClientName"];
                var contactPerson = details["SuperAdminEmail"];
                var appName = details["Name"];
                var appBaseUrl = details["ApplicationBaseUrl"];
                // create a client
                var client = new CreateClientDTO() {
                    Name = clientName,
                    ContactPersonEmail = contactPerson,
                    ClientType = ClientType.Unknown
                };
                var clientDetails = await _client.Save(client);
                // create sso  application
           }
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

    }


}

