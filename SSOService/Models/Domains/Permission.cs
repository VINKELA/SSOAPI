using SSOService.Models.Enums;
using System;

namespace SSOService.Models.Domains
{
    //permission for entities, each applications exposes entites and rights
    public class Permission : EntityTracking
    {
        public PermissionType PermissionType { get; set; }
        public Scope Scope { get; set; }
        public Entity Entity { get; set; }
        public Guid ClientId { get; set; }
        public string Name { get; set; }

    }
}
