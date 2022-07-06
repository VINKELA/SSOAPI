using SSOMachine.Models.Domains;

namespace SSOService.Models.Domains
{
    public class ApplicationPermission : EntityTracking
    {
        public long PermissionId { get; set; }
        public long ApplicationId { get; set; }
    }
}
