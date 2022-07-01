namespace SSOMachine.Models.Domains
{
    public class EntityTracking : Base
    {
        public string TrackingId { get; set; }
        public string ConcurrencyStamp { get; set; }

    }
}
