﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SSOService.Extensions;
using SSOService.Helpers;
using SSOService.Models;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Models.Domains;
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
        private readonly GetUserDTO ReturnType = new();
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<User> _logger;
        public UserRepository(IServiceResponse response, SSODbContext db, IFileRepository fileRepository,
            ILogger<User> logger, ISubscriptionRepository subscriptionRepository, IRoleRepository roleRepository)
        {
            _response = response;
            _db = db;
            _fileRepository = fileRepository;
            _subscriptionRepository = subscriptionRepository;
            _roleRepository = roleRepository;
            _logger = logger;
        }
        public async Task<Response<GetUserDTO>> CreateAsync(CreateUserDTO user)
        {
            var loggedInUser = GetLoggedInUser();
            _logger.LogInformation("Logs working");
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
                   string.Format(ValidationConstants.EntityAlreadyExist, DefaultResources.User, Email, email));
            if (string.IsNullOrEmpty(user.PhoneNumber))
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.EmptyRequiredFieldResponse, PhoneNumber));
            var phone = user.PhoneNumber.Trim().ToLower();
            if (IsPhoneNumberValid(phone))
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.InvalidFieldFormatResponse, PhoneNumber));
            if (_db.Users.Any(x => x.PhoneNumber == phone))
                return _response.FailedResponse(ReturnType,
                   string.Format(ValidationConstants.EntityAlreadyExist, DefaultResources.User, PhoneNumber, phone));
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
            var clientId = loggedInUser?.ClientId;
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
                FilePath = filePath,
                ClientId = clientId
            };
            _db.Users.Add(newUser);
            var result = await _db.SaveChangesAsync();
            var createdUser = _db.Users.FirstOrDefault(x => x.Email == user.Email);
            if (result > 0)
            {
                return _response.SuccessResponse(ToDto(createdUser));
            }
            return _response.FailedResponse(ReturnType);
        }
        public async Task<GetUserDTO> Save(CreateUserDTO user)
        {
            if (string.IsNullOrEmpty(user.FirstName))
                return null;
            var firstname = user.FirstName.Trim().ToUpper();
            if (string.IsNullOrEmpty(user.LastName))
                return null;
            var lastname = user.LastName.Trim().ToUpper();
            if (string.IsNullOrEmpty(user.Email))
                return null;
            if (!user.Email.IsValidEmailFormat())
                return null;
            var email = user.Email.Trim().ToLower();
            if (_db.Users.Any(x => x.Email == email))
                return null;
            if (string.IsNullOrEmpty(user.PhoneNumber))
                return ReturnType;
            var phone = user.PhoneNumber.Trim().ToLower();
            if (IsPhoneNumberValid(phone))
                return ReturnType;
            if (_db.Users.Any(x => x.PhoneNumber == phone))
                return null;
            if (string.IsNullOrEmpty(user.Password))
                return null;
            if (!ValidatePassword(user.Password))
                return null;
            if (string.IsNullOrEmpty(user.Confirmation))
                return null;
            if (user.Confirmation != user.Password)
                return null;
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
            var result = await _db.SaveChangesAsync();
            var createdUser = _db.Users.FirstOrDefault(x => x.Email == user.Email);
            return ToDto(createdUser);
        }

        public async Task<Response<GetUserDTO>> Update(long id, UpdateUserDTO user)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.FieldNotFound, DefaultResources.User));
            if (!string.IsNullOrEmpty(user.PhoneNumber) && !IsPhoneNumberValid(user.PhoneNumber))
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.InvalidFieldFormatResponse, PhoneNumber));
            var firstname = user.FirstName != null ? user.FirstName.Trim().ToUpper() : current.FirstName;
            var lastname = user.LastName != null ? user.LastName.Trim().ToUpper() : current.LastName;
            var username = user.UserName != null ? user.UserName.Trim().ToUpper() : current.UserName;
            long clientId = 0;
            bool hasClient;
            if (!string.IsNullOrEmpty(user.ClientId))
            {
                var client = user.ClientId.Trim();
                hasClient = long.TryParse(client, out clientId);
                var clientDetais = _db.Clients.Any(x => x.ClientId == clientId);
                if (!clientDetais) return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.InvalidFieldResponse, user.ClientId, DefaultResources.Client));
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
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.EntityChangedByAnotherUser, current.UserId));
            current.ConcurrencyStamp = Guid.NewGuid();
            _db.Users.Update(current);
            var result = await _db.SaveChangesAsync();
            if (result > 0)
            {
                _response.SuccessResponse(ToDto(current));
            }

            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> ChangeState(long id, bool deactivate = false, bool delete = false)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.FieldNotFound, DefaultResources.Client));
            if (deactivate) current.IsActive = !deactivate;
            else if (delete)
            {
                current.IsActive = !delete;
                current.IsDeleted = delete;
            }
            else current.IsActive = true;
            var hasChanged = await HasChanged(current);
            if (hasChanged)
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.EntityChangedByAnotherUser, current.UserId));
            current.ConcurrencyStamp = Guid.NewGuid();
            _db.Users.Update(current);
            var result = await _db.SaveChangesAsync();
            return result > 0 ? _response.SuccessResponse(ToDto(current)) :
            _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> Get(long id)
        {
            var current = await Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.User));
            return _response.SuccessResponse(ToDto(current));
        }
        public async Task<Response<GetUserDTO>> Get(string emailOrUsername)
        {
            var current = await GetUserByEmailOrUsername(emailOrUsername);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, DefaultResources.User));
            return _response.SuccessResponse(current);
        }
        public async Task<GetUserDTO> GetUserByEmailOrUsername(string emailOrUsername)
        {
            var current = await _db.Users.FirstOrDefaultAsync(x => x.Email == emailOrUsername
            || x.UserName == emailOrUsername);
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
                users = users.Where(x => x.FirstName.ToUpper().Contains(name)
                || x.LastName.ToUpper().Contains(name) || x.UserName.ToUpper().Contains(name));
            }
            if (!string.IsNullOrEmpty(email))
                users = users.Where(x => x.Email.Contains(email.Trim().ToLower()));
            if (!string.IsNullOrEmpty(phoneNumber))
                users = users.Where(x => x.PhoneNumber.Contains(phoneNumber.Trim()));
            if (!string.IsNullOrEmpty(client))
                users = users.Where(x => x.Client.Contains(client.Trim().ToUpper()));
            return _response.SuccessResponse(users);
        }
        public GetUserDTO GetLoggedInUser()
            => RequestContext.GetCurrentUser;
        public async Task AssignClientToUser(string code)
        {
            var client = await _db.Clients.FirstOrDefaultAsync(x => x.Code == code);
            if (client == null) return;
            var userId = GetLoggedInUser()?.Id;
            if (userId == null) return;
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null) return;
            user.ClientId = client.ClientId;
            _db.Update(user);
            await _db.SaveChangesAsync();
        }

        public async Task<Response<GetUserDTO>> AddPermission(List<long> permissions, long userId)
        {

            var permissionsExists = await _db.Permissions.AllAsync(x => permissions.Contains(x.PermissionId));
            var user = await Exists(userId);

            if (!permissionsExists)
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.FieldNotFound, DefaultResources.Permission));
            if (user == null)
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.FieldNotFound, DefaultResources.User));
            var userPermissions = new List<UserPermission>();

            foreach (var permission in permissions)
            {
                userPermissions.Add(new UserPermission
                {
                    PermissionId = permission,
                    UserId = userId,

                });
            }

            await _db.AddRangeAsync(userPermissions);
            var status = await _db.SaveChangesAsync() > 0;
            if (status) return _response.SuccessResponse(ToDto(user));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> UpdatePermission(long permissionId, long userId, bool update)
        {
            var current = await _db.UserPermissions
                .FirstOrDefaultAsync(x => x.PermissionId == permissionId && x.UserId == userId);
            if (current == null)
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.FieldNotFound, DefaultResources.User));
            current.IsActive = update ? !current.IsActive : current.IsActive;
            _db.Update(current);
            var status = await _db.SaveChangesAsync() > 0;
            if (status) return _response.SuccessResponse(ToDto(await Exists(current.UserId)));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> AddSubscription(long subscriptionId, long userId)
        {

            var subscription = await _subscriptionRepository.Get(subscriptionId);
            var user = await Exists(userId);

            if (subscription == null)
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.FieldNotFound, DefaultResources.Subscription));
            if (user == null)
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.FieldNotFound, DefaultResources.User));
            var newAuth = new UserSubscription
            {
                SubscriptionId = subscriptionId,
                UserId = userId
            };
            await _db.AddAsync(newAuth);
            var status = await _db.SaveChangesAsync() > 0;
            if (status) return _response.SuccessResponse(ToDto(user));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> UpdateSubscription(long subscriptionId, long userId, bool update)
        {

            var current = await _db.UserSubscriptions
                .FirstOrDefaultAsync(x => x.SubscriptionId == subscriptionId && x.UserId == userId);
            if (current == null)
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.FieldNotFound, DefaultResources.User));
            current.IsActive = update ? !current.IsActive : current.IsActive;
            _db.Update(current);
            var status = await _db.SaveChangesAsync() > 0;
            if (status) return _response.SuccessResponse(ToDto(await Exists(current.UserId)));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> AddRole(long roleId, long userId)
        {
            var role = await _roleRepository.Get(roleId);
            var user = await Exists(userId);

            if (role == null)
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.FieldNotFound, DefaultResources.Role));
            if (user == null)
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.FieldNotFound, DefaultResources.User));
            var newAuth = new UserRole
            {
                RoleId = roleId,
                UserId = userId
            };
            await _db.AddAsync(newAuth);
            var status = await _db.SaveChangesAsync() > 0;
            if (status) return _response.SuccessResponse(ToDto(user));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> UpdateRole(long roleId, long userId, bool update)
        {

            var current = await _db.UserRoles.FirstOrDefaultAsync(x => x.RoleId == roleId && x.UserId == userId);
            if (current == null)
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.FieldNotFound, DefaultResources.User));
            current.IsActive = update ? !current.IsActive : current.IsActive;
            _db.Update(current);
            var status = await _db.SaveChangesAsync() > 0;
            if (status) return _response.SuccessResponse(ToDto(await Exists(current.UserId)));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> AddDevice(CreateUserDevice device)
        {
            var user = await Exists(device.UserId);

            if (user == null)
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.FieldNotFound, DefaultResources.User));
            var newDevice = new UserDevice
            {
                DeviceName = device.DeviceName,
                DeviceType = device.DeviceType,
                UserId = device.UserId

            };
            await _db.AddAsync(newDevice);
            var status = await _db.SaveChangesAsync() > 0;
            if (status) return _response.SuccessResponse(ToDto(user));
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> AddLogin(CreateUserLogin login)
        {
            var user = await Exists(login.UserId);

            if (user == null)
                return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.FieldNotFound, DefaultResources.User));
            var newDevice = new UserLogin
            {
                LoginProvider = login.LoginProvider,
                ProdviderDisplayName = login.ProdviderDisplayName,
                ProviderKey = login.ProviderKey,
                UserId = login.UserId
            };
            await _db.AddAsync(newDevice);
            var status = await _db.SaveChangesAsync() > 0;
            if (status) return _response.SuccessResponse(ToDto(user));
            return _response.FailedResponse(ReturnType);
        }

        private GetUserDTO ToDto(User user)
        {
            if (user == null) return null;
            var roleData = new List<UserRoleDTO>();
            var client = _db.Clients.FirstOrDefault(x => x.ClientId == user.ClientId);
            var roles = _db.UserRoles.Where(x => x.UserId == user.UserId).ToList();
            var userRoles = _db.Roles.Where(x => roles.Select(y => y.RoleId).Contains(x.RoleId)).ToList();
            roleData.AddRange(userRoles.Select(x => new UserRoleDTO
            {
                Code = x.Code,
                RoleName = x.Name
            }));
            var clientId = client.ClientId;
            var clientName = client?.Name;
            return new GetUserDTO()
            {
                FirstName = user.FirstName.ToTitleCase(),
                LastName = user.LastName.ToTitleCase(),
                UserName = user.UserName.ToTitleCase(),
                Email = user.Email.ToLower(),
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                Client = clientName,
                ClientId = clientId,
                Id = user.UserId,
                Image = user.FilePath != null ?
                _fileRepository.Get(user.FilePath, FileType.UserImage) : null,
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
        private async Task<User> Exists(long id)
        {
            var current = await _db.Users.FirstOrDefaultAsync(x => x.UserId == id && !x.IsDeleted);
            return current;
        }
        private async Task<bool> HasChanged(User user)
        {
            var lastest = await Exists(user.UserId);
            return !(user.ConcurrencyStamp == lastest.ConcurrencyStamp);
        }
        private static bool IsPhoneNumberValid(string phoneNumber)
        {
            var re = PhonefNumberValidation;

            return Regex.IsMatch(phoneNumber, re);
        }
    }
}
