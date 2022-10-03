using System;

namespace SSOService.Models.DTOs.Role
{
    public class GetRoleClaim
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public long RoleId { get; set; }
        public string Role { get; set; }

    }
}
