using System;

namespace SSOService.Models.Domains
{
    // These are services within a subscription
    public class SubscriptionResource : EntityTracking
    {
        public long SubscriptionResourceId { get; set; }
        public long SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }
        public long ResourceId { get; set; }
        public Resource Resource { get; set; }
    }
}
