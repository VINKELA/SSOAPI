using Microsoft.EntityFrameworkCore;
using SSOMachine.Models.Domains;
using SSOService.Extensions;
using SSOService.Helpers;
using SSOService.Models;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Models.DTOs.User;
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
        private readonly GetUserDTO ReturnType = new();
        public UserRepository(IServiceResponse response, SSODbContext db)
        {
            _response = response;
            _db = db;
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
            if (!IsPhoneNumberValid(phone))
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
            Guid cliendId = Guid.Empty;
            bool hasClient;
            if (!string.IsNullOrEmpty(user.ClientId))
            {
                hasClient = Guid.TryParse(user.ClientId, out cliendId);
                var clientDetais = _db.Clients.Any(x => x.Id == cliendId);
                if (!clientDetais) return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.InvalidFieldResponse, user.ClientId, ClassNames.Client));
            }
            var username = !string.IsNullOrEmpty(user.UserName) ? user.UserName.Trim().ToUpper()
                : firstname;
            var newUser = new User()
            {
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                UserName = username,
                ClientId = cliendId != Guid.Empty ? cliendId : null,
                PasswordHash = HashEngine.GetHash(user.Password),
                PhoneNumber = phone
            };
            _db.Users.Add(newUser);
            var result = await _db.SaveAndAuditChangesAsync(Guid.NewGuid());
            return result > 0 ? _response.SuccessResponse(Todto(newUser)) :
                _response.FailedResponse(ReturnType);
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
            bool hasClient;
            if (!string.IsNullOrEmpty(user.ClientId))
            {
                hasClient = Guid.TryParse(user.ClientId, out cliendId);
                var clientDetais = _db.Clients.Any(x => x.Id == cliendId);
                if (!clientDetais) return _response.FailedResponse(ReturnType,
                    string.Format(ValidationConstants.InvalidFieldResponse, user.ClientId, ClassNames.Client));
            }
            current.FirstName = firstname;
            current.LastName = lastname;
            current.UserName = username;
            current.ClientId = cliendId == Guid.Empty ? current.ClientId : cliendId;
            current.Modified = DateTime.Now;
            if (HasChanged(current))
                return _response.FailedResponse(ReturnType, string.Format(ValidationConstants.EntityChangedByAnotherUser, current.Id));
            current.ConcurrencyStamp = Guid.NewGuid();
            _db.Users.Update(current);
            var result = await _db.SaveAndAuditChangesAsync(Guid.NewGuid());
            return result > 0 ? _response.SuccessResponse(Todto(current)) :
            _response.FailedResponse(ReturnType);
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
                users = users.Where(x => x.ClientName.ToUpper().Contains(client.Trim().ToUpper()));
            return _response.SuccessResponse(users);
        }
        private GetUserDTO Todto(User user)
        {
            if (user.Id == Guid.Empty)
                user = _db.Users.FirstOrDefault(x => x.Email == user.Email);
            var client = _db.Clients.FirstOrDefault(x => x.Id == user.ClientId);
            var clientName = client != null ? client.Name : ValidationConstants.NotAvailable;
            return new GetUserDTO()
            {
                FirstName = user.FirstName.ToTitleCase(),
                LastName = user.LastName.ToTitleCase(),
                UserName = user.UserName.ToTitleCase(),
                Email = user.Email.ToLower(),
                PhoneNumber = user.PhoneNumber,
                ClientName = clientName,
                IsActive = user.IsActive,
                ClientId = user.ClientId,
                Id = user.Id
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
            return Regex.Match(phoneNumber, @"^(\+[0-9]{9})$").Success;
        }


    }
}
