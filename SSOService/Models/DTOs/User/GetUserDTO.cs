using SSOService.Models.DTOs.Client;
using System;
using System.Collections.Generic;

namespace SSOService.Models.DTOs.User
{
    public class GetUserDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public List<UserClientDTO> Clients { get; set; }
        public bool IsActive { get; set; }
        public byte[] Image { get; set; }
        public List<UserRoleDTO> UserRoles { get; set; }

    }
}

