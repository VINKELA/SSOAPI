using SSOMachine.Models.Domains;

namespace SSOService.Models.Domains
{
    public class UserSubscription : EntityTracking
    {
        public long UserId { get; set; }
        public long SubscriptionId { get; set; }
    }
}
