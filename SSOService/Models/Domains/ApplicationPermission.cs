using System;

namespace SSOService.Models.Domains
{
    //This refers to the permission granted form one app to another on Entities
    public class ApplicationPermission : EntityTracking
    {
        public long ApplicationPermissionId { get; set; }
        public long PermissionId { get; set; }
        public Permission Permission { get; set; }
        public long ApplicationId { get; set; }
        public Application Application { get; set; }
    }
}
