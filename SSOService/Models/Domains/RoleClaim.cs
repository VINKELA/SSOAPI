using System;

namespace SSOMachine.Models.Domains
{
    public class RoleClaim : EntityTracking
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public Guid RoleId { get; set; }
    }
}
