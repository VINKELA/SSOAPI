using System;

namespace SSOService.Models.Domains
{

    public class UserSubscription : EntityTracking
    {
        public long UserSubscriptionId { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }  

        public long SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }  
    }
}
