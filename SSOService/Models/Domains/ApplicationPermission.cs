using SSOMachine.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    //This refers to the permission granted form one app to another on Entities
    public class ApplicationPermission : Base
    {
        public Guid PermissionId { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
