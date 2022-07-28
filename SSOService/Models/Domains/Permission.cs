using SSOMachine.Models.Domains;
using SSOService.Models.Enums;
using System;

namespace SSOService.Models.Domains
{
    //permission for entities, each applications exposes entites and rights
    public class Permission : EntityTracking
    {
        public string Name { get; set; }
        public string Entity { set; get; }
        public string EntityId { get; set; }
        public PermissionType PermissionType { get; set; }
        public Guid? ApplicationId { get; set; }
    }
}
