using SSOMachine.Models.Domains;
using System;

namespace SSOService.Models.Domains
{

    public class UserRole : Base
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
