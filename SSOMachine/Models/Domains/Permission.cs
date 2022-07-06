using SSOMachine.Models.Domains;
using SSOService.Models.Enums;

namespace SSOService.Models.Domains
{
    public class Permission : EntityTracking
    {
        public string Name { get; set; }
        public string Entity { set; get; }
        public string EntityId { get; set; }
        public PermissionType PermissionType { get; set; }
    }
}
