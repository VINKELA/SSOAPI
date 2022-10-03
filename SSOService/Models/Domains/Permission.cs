using SSOService.Models.Enums;
using System;

namespace SSOService.Models.Domains
{
    //permission for entities, each applications exposes entites and rights
    public class Permission : EntityTracking
    {
        public long PermissionId { get; set; }
        public PermissionType PermissionType { get; set; }
        public Scope Scope { get; set; }
        public long ResourceId { get; set; }
        public Resource Resource { get; set; }
        public string Name { get; set; }
    }
}
