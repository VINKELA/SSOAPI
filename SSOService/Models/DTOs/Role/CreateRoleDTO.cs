using System;

namespace SSOService.Models.DTOs.Role
{
    public class CreateRoleDTO
    {
        public string Name { get; set; }
        public Guid ClientId { get; set; }
    }
}
