using SSOMachine.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    public class SubscriptionServices : EntityTracking
    {
        public Guid SubscriptionId { get; set; }
        public Guid ServiceId { get; set; }
    }
}
