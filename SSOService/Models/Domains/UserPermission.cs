using SSOMachine.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    //This is the user permissions
    public class UserPermission : Base
    {
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }

    }
}
