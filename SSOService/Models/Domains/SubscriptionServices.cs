using SSOService.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    // These are services within a subscription
    public class SubscriptionServices : Base
    {
        public Guid SubscriptionId { get; set; }
        public Guid ServiceId { get; set; }
    }
}
