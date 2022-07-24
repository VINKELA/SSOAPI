using SSOMachine.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    public class UserSubscription : EntityTracking
    {
        public Guid UserId { get; set; }
        public Guid SubscriptionId { get; set; }
    }
}
