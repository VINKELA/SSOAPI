using System;

namespace SSOService.Models.DTOs.User
{
    public class UserPermissionDTO
    {
        public string PermissionName { get; set; }
        public Guid ClientId { get; set; }
        public Guid PermissionId { get; set; }
    }


}
