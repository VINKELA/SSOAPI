using System;

namespace SSOService.Models.Domains
{
    // These are services within a subscription
    public class SubscriptionResource : Base
    {
        public Guid SubscriptionId { get; set; }
        public Guid ResourceId { get; set; }
    }
}
