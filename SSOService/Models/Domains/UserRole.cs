using SSOService.Models.Domains;
using System;

namespace SSOService.Models.Domains
{

    public class UserRole : EntityTracking
    {
        public long UserRoleId { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public long RoleId { get; set; }
        public Role Role { get; set; }
    }
}
