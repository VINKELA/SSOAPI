using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SSOService.Extensions;
using SSOService.Helpers;
using SSOService.Models;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Models.Domains;
using SSOService.Models.DTOs.Client;
using SSOService.Models.DTOs.User;
using SSOService.Models.Enums;
using SSOService.Services.General.Interfaces;
using SSOService.Services.Repositories.NonRelational.Interfaces;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.Relational.Implementations
{
    public class UserRepository : IUserRepository
    {
        private const string Email = "Email";
        private const string FirstName = "First Name";
        private const string LastName = "Last Name";
        private const string PhoneNumber = "Phone number";
        private const string PasswordConfirmationMismatch = "Password and Password Confirmation do not match";
        private const string Password = "Password";
        private const string Confirmation = "Confirmation";
        private const string PasswordValidation = "password must be 7 character long, " +
            "contain lowercase, upper case and special character";
        private const string PhonefNumberValidation = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
        private readonly SSODbContext _db;
        private readonly IServiceResponse _response;
        private readonly IFileRepository _fileRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly GetUserDTO ReturnType = new();
        private readonly IPermissionRepository _permissionRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IRoleRepository _roleRepository;
        public UserRepository(IServiceResponse response, SSODbContext db, IFileRepository fileRepository,
            IHttpContextAccessor httpContext, IPermissionRepository permissionRepository, ISubscriptionRepository subscriptionRepository, IRoleRepository roleRepository)
        {
            _response = response;
            _db = db;
            _fileRepository = fileRepository;
            _httpContext = httpContext;
            _permissionRepository = permissionRepository;
            _subscriptionRepository = subscriptionRepository;
            _roleRepository = roleRepository;
        }
        public async Task<Response<GetUserDTO>> Save(CreateUserDTO user)
        {
            if (string.IsNullOrEmpty(user.FirstName))
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.EmptyRequiredFieldResponse, FirstName));
            var firstname = user.FirstName.Trim().ToUpper();
            if (string.IsNullOrEmpty(user.LastName))
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.EmptyRequiredFieldResponse, LastName));
            var lastname = user.LastName.Trim().ToUpper();
            if (string.IsNullOrEmpty(user.Email))
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.EmptyRequiredFieldResponse, Email));
            if (!user.Email.IsValidEmailFormat())
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.InvalidFieldFormatResponse, Email));
            var email = user.Email.Trim().ToLower();
            if (_db.Users.Any(x => x.Email == email))
                return _response.FailedResponse(ReturnType,
                   string.Format(ValidationConstants.EntityAlreadyExist, ClassNames.User, Email, email));
            if (string.IsNullOrEmpty(user.PhoneNumber))
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.EmptyRequiredFieldResponse, PhoneNumber));
            var phone = user.PhoneNumber.Trim().ToLower();
            if (IsPhoneNumberValid(phone))
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.InvalidFieldFormatResponse, PhoneNumber));
            if (_db.Users.Any(x => x.PhoneNumber == phone))
                return _response.FailedResponse(ReturnType,
                   string.Format(ValidationConstants.EntityAlreadyExist, ClassNames.User, PhoneNumber, phone));
            if (string.IsNullOrEmpty(user.Password))
                return _response.FailedResponse(ReturnType,
                string.Format(ValidationConstants.EmptyRequiredFieldResponse, Password));
            if (!ValidatePassword(user.Password))
                return _response.FailedResponse(ReturnType, PasswordValidation);
            if (string.IsNullOrEmpty(user.Confirmation))
                return _response.FailedResponse(ReturnType,
                string.Format(ValidationConstants.EmptyRequiredFieldResponse, Confirmation));
            if (user.Confirmation != user.Password)
                return _response.FailedResponse(ReturnType, PasswordConfirmationMismatch);
            List<Guid> clients = new();
            bool hasClient;
            if (user.Clients != null && user.Clients.Count > 0)
            {
                for (int i = 0; i < user.Clients.Count; i++)
                {
                    var client = user.Clients[i];
                    hasClient = Guid.TryParse(client, out Guid clientGuid);
                    var clientDetais = _db.Clients.Any(x => x.Id == clientGuid);
                    if (!clientDetais) return _response.FailedResponse(ReturnType,
                        string.Format(ValidationConstants.InvalidFieldResponse, user.Clients, ClassNames.Client));
                    clients.Add(clientGuid);
                }
            }
            string filePath = null;
            if (user.File != null)
            {
                filePath = await _fileRepository.Save(user.File, email, FileType.UserImage);
            }
            var username = !string.IsNullOrEmpty(user.UserName) ? user.UserName.Trim().ToUpper()
                : firstname;
            var newUser = new User()
            {
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                UserName = username,
                PasswordHash = HashEngine.GetHash(user.Password),
                PhoneNumber = phone,
                FilePath = filePath

            };
            _db.Users.Add(newUser);
            var result = await _db.SaveAndAuditChangesAsync(GetLoggedInUser()?.Id);
            var createdUser = _db.Users.FirstOrDefault(x => x.Email == user.Email);
            if (createdUser != null)
            {
                if (clients.Count > 0)
                    await RegisterUserWithClient(createdUser.Id, clients);
                return _response.SuccessResponse(ToDto(createdUser));
            }
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> Update(Guid id, UpdateUserDTO user)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            if (!string.IsNullOrEmpty(user.PhoneNumber) && !IsPhoneNumberValid(user.PhoneNumber))
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.InvalidFieldFormatResponse, PhoneNumber));
            var firstname = user.FirstName != null ? user.FirstName.Trim().ToUpper() : current.FirstName;
            var lastname = user.LastName != null ? user.LastName.Trim().ToUpper() : current.LastName;
            var username = user.UserName != null ? user.UserName.Trim().ToUpper() : current.UserName;
            Guid cliendId = Guid.Empty;
            List<Guid> clients = new();
            bool hasClient;
            if (user.ClientIds.Count > 0)
            {
                for (int i = 0; i < user.ClientIds.Count; i++)
                {
                    var client = user.ClientIds[i];
                    hasClient = Guid.TryParse(client, out Guid clientGuid);
                    var clientDetais = _db.Clients.Any(x => x.Id == clientGuid);
                    if (!clientDetais) return _response.FailedResponse(ReturnType,
                        string.Format(ValidationConstants.InvalidFieldResponse, client, ClassNames.Client));
                    clients.Add(clientGuid);
                }
            }
            string filePath = current.FilePath;
            if (user.File != null)
            {
                filePath = await _fileRepository.Save(user.File, current.Email, FileType.UserImage);
            }
            current.FirstName = firstname;
            current.LastName = lastname;
            current.UserName = username;
            current.Modified = DateTime.Now;
            current.FilePath = filePath;
            var hasChanged = await HasChanged(current);
            if (hasChanged)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.EntityChangedByAnotherUser, current.Id));
            current.ConcurrencyStamp = Guid.NewGuid();
            _db.Users.Update(current);
            var result = await _db.SaveAndAuditChangesAsync();
            if (result > 0)
            {
                if (clients.Count > 0)
                    await RegisterUserWithClient(current.Id, clients);
                _response.SuccessResponse(ToDto(current));
            }

            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Client));
            if (deactivate) current.IsActive = !deactivate;
            else if (delete)
            {
                current.IsActive = !delete;
                current.IsDeleted = delete;
            }
            else current.IsActive = true;
            var hasChanged = await HasChanged(current);
            if (hasChanged)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.EntityChangedByAnotherUser, current.Id));
            current.ConcurrencyStamp = Guid.NewGuid();
            _db.Users.Update(current);
            var result = await _db.SaveAndAuditChangesAsync(GetLoggedInUser().Id);
            return result > 0 ? _response.SuccessResponse(ToDto(current)) :
            _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> Get(Guid id)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            return _response.SuccessResponse(ToDto(current));
        }
        public async Task<Response<GetUserDTO>> Get(string emailOrUsername)
        {
            var current = await GetUserByEmailOrUsername(emailOrUsername);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            return _response.SuccessResponse(current);
        }
        public async Task<GetUserDTO> GetUserByEmailOrUsername(string emailOrUsername)
        {
            var current = await _db.Users.FirstOrDefaultAsync(x => x.Email == emailOrUsername || x.UserName == emailOrUsername);
            return ToDto(current);
        }
        public async Task<Response<IEnumerable<GetUserDTO>>> Get(string name, string email,
            string phoneNumber, string client)
        {
            var user = GetLoggedInUser();
            var list = await _db.Users.Where(x => !x.IsDeleted).ToListAsync();
            var users = list.Select(x => ToDto(x));
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim().ToUpper();
                users = users.Where(x => x.FirstName.ToUpper().Contains(name) || x.LastName.ToUpper().Contains(name) || x.UserName.ToUpper().Contains(name));
            }
            if (!string.IsNullOrEmpty(email))
                users = users.Where(x => x.Email.Contains(email.Trim().ToLower()));
            if (!string.IsNullOrEmpty(phoneNumber))
                users = users.Where(x => x.PhoneNumber.Contains(phoneNumber.Trim()));
            if (!string.IsNullOrEmpty(client))
                users = users.Where(x => x.Clients.Select(x => x.ClientName).Contains(client.Trim().ToUpper()));
            return _response.SuccessResponse(users);
        }
        public GetUserDTO GetLoggedInUser()
            => (GetUserDTO)_httpContext.HttpContext.Items[HttpConstants.CurrentUser];


        public async Task<Response<GetUserDTO>> AddPermission(Guid permissionId, Guid userId)
        {
            var loggedInUser = GetLoggedInUser();

            var permission = await _permissionRepository.Get(permissionId);
            var user = await Exists(userId);

            if (permission == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Permission));
            if (user == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            var newPermission = new UserPermission
            {
                PermissionId = permissionId,
                UserId = userId
            };
            await _db.AddAsync(newPermission);
            var status = await _db.SaveAndAuditChangesAsync(loggedInUser.Id) > 0;
            if (status) return _response.SuccessResponse(ToDto(user));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> UpdatePermission(Guid permissionId, Guid userId, bool update)
        {
            var user = GetLoggedInUser();
            var current = await _db.UserPermissions.FirstOrDefaultAsync(x => x.PermissionId == permissionId && x.UserId == userId);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            current.IsActive = update ? !current.IsActive : current.IsActive;
            _db.Update(current);
            var status = await _db.SaveAndAuditChangesAsync(user.Id) > 0;
            if (status) return _response.SuccessResponse(ToDto(await Exists(current.UserId)));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> AddSubscription(Guid subscriptionId, Guid userId)
        {
            var loggedInuser = GetLoggedInUser();

            var subscription = await _subscriptionRepository.Get(subscriptionId);
            var user = await Exists(userId);

            if (subscription == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Subscription));
            if (user == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            var newAuth = new UserSubscription
            {
                SubscriptionId = subscriptionId,
                UserId = userId
            };
            await _db.AddAsync(newAuth);
            var status = await _db.SaveAndAuditChangesAsync(loggedInuser.Id) > 0;
            if (status) return _response.SuccessResponse(ToDto(user));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> UpdateSubscription(Guid subscriptionId, Guid userId, bool update)
        {
            var loggedInUser = GetLoggedInUser();
            var current = await _db.UserSubscriptions.FirstOrDefaultAsync(x => x.SubscriptionId == subscriptionId && x.UserId == userId);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            current.IsActive = update ? !current.IsActive : current.IsActive;
            _db.Update(current);
            var status = await _db.SaveAndAuditChangesAsync(loggedInUser.Id) > 0;
            if (status) return _response.SuccessResponse(ToDto(await Exists(current.UserId)));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> AddRole(Guid roleId, Guid userId)
        {
            var loggedInuser = GetLoggedInUser();

            var role = await _roleRepository.Get(roleId);
            var user = await Exists(userId);

            if (role == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Role));
            if (user == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            var newAuth = new UserRole
            {
                RoleId = roleId,
                UserId = userId
            };
            await _db.AddAsync(newAuth);
            var status = await _db.SaveAndAuditChangesAsync(loggedInuser.Id) > 0;
            if (status) return _response.SuccessResponse(ToDto(user));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> UpdateRole(Guid roleId, Guid userId, bool update)
        {
            var loggedInUser = GetLoggedInUser();
            var current = await _db.UserRoles.FirstOrDefaultAsync(x => x.RoleId == roleId && x.UserId == userId);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            current.IsActive = update ? !current.IsActive : current.IsActive;
            _db.Update(current);
            var status = await _db.SaveAndAuditChangesAsync(loggedInUser.Id) > 0;
            if (status) return _response.SuccessResponse(ToDto(await Exists(current.UserId)));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> AddDevice(CreateUserDevice device)
        {
            var loggedInuser = GetLoggedInUser();

            var user = await Exists(device.UserId);

            if (user == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            var newDevice = new UserDevice
            {
                DeviceName = device.DeviceName,
                DeviceType = device.DeviceType,
                UserId = device.UserId

            };
            await _db.AddAsync(newDevice);
            var status = await _db.SaveAndAuditChangesAsync(loggedInuser.Id) > 0;
            if (status) return _response.SuccessResponse(ToDto(user));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> AddLogin(CreateUserLogin login)
        {
            var loggedInuser = GetLoggedInUser();

            var user = await Exists(login.UserId);

            if (user == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            var newDevice = new UserLogin
            {
                LoginProvider = login.LoginProvider,
                ProdviderDisplayName = login.ProdviderDisplayName,
                ProviderKey = login.ProviderKey,
                UserId = login.UserId
            };
            await _db.AddAsync(newDevice);
            var status = await _db.SaveAndAuditChangesAsync(loggedInuser.Id) > 0;
            if (status) return _response.SuccessResponse(ToDto(user));
            return _response.FailedResponse(ReturnType);
        }

        private GetUserDTO ToDto(User user)
        {
            if (user == null) return null;
            var data = new List<UserClientDTO>();
            var roleData = new List<UserRoleDTO>();
            var clients = _db.Clients.Where(x => !x.IsDeleted);
            var userClients = _db.UserClients.Where(x => x.UserId == user.Id).ToList();
            var roles = _db.UserRoles.Where(x => x.UserId == user.Id).ToList();
            var userRoles = _db.Roles.Where(x => roles.Select(y => y.Id).Contains(x.Id)).ToList();
            roleData.AddRange(userRoles.Select(x => new UserRoleDTO
            {
                Code = x.Code,
                RoleName = x.Name
            }));
            foreach (var client in userClients)
            {
                var currentClient = clients.FirstOrDefault(y => y.Id == client.ClientId);
                if (currentClient != null)
                {
                    data.Add(new UserClientDTO
                    {
                        ClientId = currentClient.Id,
                        ClientName = currentClient.Name
                    });
                }
            }

            return new GetUserDTO()
            {
                FirstName = user.FirstName.ToTitleCase(),
                LastName = user.LastName.ToTitleCase(),
                UserName = user.UserName.ToTitleCase(),
                Email = user.Email.ToLower(),
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                Clients = data,
                Id = user.Id,
                Image = user.FilePath != null ? _fileRepository.Get(user.FilePath, FileType.UserImage) : null,
                PasswordHash = user.PasswordHash,
                UserRoles = roleData
            };
        }
        private static bool ValidatePassword(string password)
        {
            var minimumLength = 7;
            var count = password.Length;
            if (count < minimumLength) return false;
            var upperCase = false;
            var lowerCase = false;
            var digit = false;
            var specialCharacter = false;
            var valid = false;
            var reachedEnd = false;
            var i = 0;
            while (!valid && !reachedEnd)
            {
                var current = password[i];
                upperCase = upperCase ? upperCase : char.IsUpper(current);
                lowerCase = lowerCase ? lowerCase : char.IsLower(current);
                digit = digit ? digit : char.IsNumber(current);
                specialCharacter = specialCharacter ? specialCharacter : (char.IsSymbol(current)
                    || char.IsControl(current) || char.IsPunctuation(current));
                valid = upperCase && digit && specialCharacter && lowerCase;
                i++;
                reachedEnd = i == count;

            }
            return valid;
        }
        private async Task<User> Exists(Guid id)
        {
            var current = await _db.Users.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            return current;
        }
        private async Task<bool> HasChanged(User user)
        {
            var lastest = await Exists(user.Id);
            return !(user.ConcurrencyStamp == lastest.ConcurrencyStamp);
        }
        private static bool IsPhoneNumberValid(string phoneNumber)
        {
            var re = PhonefNumberValidation;

            return Regex.IsMatch(phoneNumber, re);
        }
        private async Task<bool> RegisterUserWithClient(Guid userId, List<Guid> clients)
        {
            _db.UserClients.AddRange(clients
               .Select(x => new UserClient { UserId = userId, ClientId = x }));
            var result = await _db.SaveAndAuditChangesAsync(GetLoggedInUser().Id);
            return result > 0;
        }
    }
}
