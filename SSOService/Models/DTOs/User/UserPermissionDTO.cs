using System;

namespace SSOService.Models.DTOs.User
{
    public class UserPermissionDTO
    {
        public string PermissionName { get; set; }
        public long ClientId { get; set; }
        public long PermissionId { get; set; }
    }


}
