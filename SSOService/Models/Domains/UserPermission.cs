using SSOMachine.Models.Domains;

namespace SSOService.Models.Domains
{
    //This is the user permissions
    public class UserPermission : Base
    {
        public long UserId { get; set; }
        public long PermissionId { get; set; }

    }
}
