using SSOMachine.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    public class ApplicationPermission : EntityTracking
    {
        public Guid PermissionId { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
