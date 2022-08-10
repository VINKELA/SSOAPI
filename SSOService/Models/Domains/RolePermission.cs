using SSOService.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    // permission to entites for roles
    public class RolePermission : Base
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
