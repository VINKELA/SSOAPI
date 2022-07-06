using System;

namespace SSOMachine.Models.Domains
{
    public class ClientSubscription : EntityTracking
    {
        public long ClientId { get; set; }
        public long SubscriptionId { get; set; }
        public DateTime ActivatedOn { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
