using System;

namespace SSOService.Models.Domains
{
    //A client can create custom permission for users using role claim
    public class RoleClaim : EntityTracking
    {
        public long RoleClaimId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public long RoleId { get; set; }
        public Role Role { get; set; }
    }
}
