using System;

namespace SSOMachine.Models.Domains
{
    public class ClientSubscription : EntityTracking
    {
        public Guid ClientId { get; set; }
        public Guid SubscriptionId { get; set; }
        public DateTime ActivatedOn { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
