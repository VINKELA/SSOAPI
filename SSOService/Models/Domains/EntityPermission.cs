using SSOMachine.Models.Domains;
using SSOService.Models.Enums;
using System;

namespace SSOService.Models.Domains
{
    //permission for entities, each applications exposes entites and rights
    public class EntityPermission : EntityTracking
    {
        public Guid EntityId { get; set; }
        public PermissionType PermissionType { get; set; }
    }
}
