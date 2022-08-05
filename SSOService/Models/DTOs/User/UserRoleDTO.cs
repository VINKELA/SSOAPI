using System;

namespace SSOService.Models.DTOs.User
{
    public class UserRoleDTO
    {
        public string RoleName { get; set; }
        public Guid ClientId { get; set; }
        public Guid RoleId { get; set; }
    }
}