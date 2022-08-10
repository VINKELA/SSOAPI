using SSOService.Models.Domains;
using System;

namespace SSOService.Models.Domains
{

    public class UserSubscription : Base
    {
        public Guid UserId { get; set; }
        public Guid SubscriptionId { get; set; }
    }
}
