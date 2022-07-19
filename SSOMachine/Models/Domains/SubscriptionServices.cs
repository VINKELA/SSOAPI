using SSOMachine.Models.Domains;

namespace SSOService.Models.Domains
{
    public class SubscriptionServices : EntityTracking
    {
        public long SubscriptionId { get; set; }
        public long ServiceId { get; set; }
    }
}
