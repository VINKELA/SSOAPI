using SSOMachine.Models.Domains;

namespace SSOService.Models.Domains
{
    // permission to entites for roles
    public class RolePermission : Base
    {
        public long RoleId { get; set; }
        public long PermissionId { get; set; }
    }
}
