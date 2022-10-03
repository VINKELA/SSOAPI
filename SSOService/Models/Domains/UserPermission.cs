using SSOService.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    //This is the user permissions
    public class UserPermission : EntityTracking
    {
        public long UserPermissionId { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }

        public long PermissionId { get; set; }
        public Permission permission { get; set; }

    }
}
