using SSOService.Models.Enums;

namespace SSOService.Models.DTOs.Permission
{
    public class UpdatePermissionDTO
    {
        public string Name { get; set; }

        public PermissionType PermissionType { get; set; }
        public Scope Scope { get; set; }
        public Entity Entity { get; set; }
    }
}
