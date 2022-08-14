using System;

namespace SSOService.Models.Domains
{
    //This refers to the suscriptions by a customer or client
    public class ClientSubscription : EntityTracking
    {
        public ClientSubscription()
        {
            ActivatedOn = DateTime.Now;
        }
        public Guid ClientId { get; set; }
        public Guid SubscriptionId { get; set; }
        public DateTime ActivatedOn { get; set; }
        public DateTime? ExpiredOn { get; set; }
    }
}
