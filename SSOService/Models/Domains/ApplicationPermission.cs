using System;

namespace SSOService.Models.Domains
{
    //This refers to the permission granted form one app to another on Entities
    public class ApplicationPermission : EntityTracking
    {
        public Guid PermissionId { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
