namespace SSOMachine.Models.Domains
{
    public class RoleClaim : EntityTracking
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public int RoleId { get; set; }
    }
}
