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
        public long ClientSubscriptionId { get; set; }
        public long ClientId { get; set; }
        public Client Client { get; set; }
        public long SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }  
        public DateTime ActivatedOn { get; set; }
        public DateTime? ExpiredOn { get; set; }
    }
}
