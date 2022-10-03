using System;

namespace SSOService.Models.DTOs.Role
{
    public class CreateRoleClaim
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public long RoleId { get; set; }

    }
}
