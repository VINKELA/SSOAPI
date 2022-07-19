using SSOMachine.Models.Domains;

namespace SSOService.Models.Domains
{
    public class UserPermission : EntityTracking
    {
        public long UserId { get; set; }
        public long PermissionId { get; set; }

    }
}
