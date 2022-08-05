using Microsoft.EntityFrameworkCore;
using SSOMachine.Models.Domains;
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
        private readonly SSODbContext _db;
        private readonly IServiceResponse _response;
        private readonly IFileRepository _fileRepository;
        private readonly GetUserDTO ReturnType = new();
        public UserRepository(IServiceResponse response, SSODbContext db, IFileRepository fileRepository)
        {
            _response = response;
            _db = db;
            _fileRepository = fileRepository;
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
            var result = await _db.SaveAndAuditChangesAsync(Guid.NewGuid());
            var createdUser = _db.Users.FirstOrDefault(x => x.Email == user.Email);
            if (createdUser != null)
            {
                if (clients.Count > 0)
                    await RegisterUserWithClient(createdUser.Id, clients);
                return _response.SuccessResponse(Todto(createdUser));
            }
            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> Update(Guid id, UpdateUserDTO user)
        {
            var current = Exists(id);
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
            if (HasChanged(current))
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.EntityChangedByAnotherUser, current.Id));
            current.ConcurrencyStamp = Guid.NewGuid();
            _db.Users.Update(current);
            var result = await _db.SaveAndAuditChangesAsync(Guid.NewGuid());
            if (result > 0)
            {
                if (clients.Count > 0)
                    await RegisterUserWithClient(current.Id, clients);
                _response.SuccessResponse(Todto(current));
            }

            return _response.FailedResponse(ReturnType);
        }
        public async Task<Response<GetUserDTO>> ChangeState(Guid id, bool deactivate = false, bool delete = false)
        {
            var current = Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.Client));
            if (deactivate) current.IsActive = !deactivate;
            else if (delete)
            {
                current.IsActive = !delete;
                current.IsDeleted = delete;
            }
            else current.IsActive = true;
            if (HasChanged(current))
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.EntityChangedByAnotherUser, current.Id));
            current.ConcurrencyStamp = Guid.NewGuid();
            _db.Users.Update(current);
            var result = await _db.SaveAndAuditChangesAsync(Guid.NewGuid());
            return result > 0 ? _response.SuccessResponse(Todto(current)) :
            _response.FailedResponse(ReturnType);
        }
        public Response<GetUserDTO> Get(Guid id)
        {
            var current = Exists(id);
            if (current == null)
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.FieldNotFound, ClassNames.User));
            return _response.SuccessResponse(Todto(current));
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
            return Todto(current);
        }

        public async Task<Response<IEnumerable<GetUserDTO>>> Get(string name, string email,
            string phoneNumber, string client)
        {
            var list = await _db.Users.Where(x => !x.IsDeleted).ToListAsync();
            var users = list.Select(x => Todto(x));
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
        private GetUserDTO Todto(User user)
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
                RoleId = x.Id,
                ClientId = x.ClientId,
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
        private User Exists(Guid id)
        {
            var current = _db.Users.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            return current;
        }
        private bool HasChanged(User user)
        {
            var lastest = Exists(user.Id);
            return !(user.ConcurrencyStamp == lastest.ConcurrencyStamp);
        }
        private static bool IsPhoneNumberValid(string phoneNumber)
        {
            var re = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";

            return Regex.IsMatch(phoneNumber, re);
        }
        private async Task<bool> RegisterUserWithClient(Guid userId, List<Guid> clients)
        {
            _db.UserClients.AddRange(clients
               .Select(x => new UserClient { UserId = userId, ClientId = x }));
            var result = await _db.SaveAndAuditChangesAsync(Guid.NewGuid());
            return result > 0;
        }


    }
}
