using SSOService.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    // permission to entites for roles
    public class RolePermission : EntityTracking
    {
        public long RolePermissionId { get; set; }
        public long RoleId { get; set; }
        public Role Role { get; set; }
        public long PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}
