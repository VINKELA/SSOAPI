namespace SSOMachine.Models.Domains
{
    public class Role : EntityTracking
    {
        public string Name { get; set; }
        public long ClientId { get; set; }
    }
}
