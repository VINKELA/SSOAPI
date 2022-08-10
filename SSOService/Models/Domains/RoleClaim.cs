using System;

namespace SSOService.Models.Domains
{
    //A client can create custom permission for users using role claim
    public class RoleClaim : Base
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public Guid RoleId { get; set; }
    }
}
