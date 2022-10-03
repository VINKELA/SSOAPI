using System;

namespace SSOService.Models.DTOs.Role
{
    public class CreateRoleDTO
    {
        public string Name { get; set; }
        public long ClientId { get; set; }
    }
}
