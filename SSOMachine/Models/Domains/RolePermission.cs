using SSOMachine.Models.Domains;

namespace SSOService.Models.Domains
{
    public class RolePermission : EntityTracking
    {
        public long RoleId { get; set; }
        public long PermissionId { get; set; }
    }
}
