using System;

namespace SSOService.Models.DTOs.Subscription
{
    public class GetSubscriptionDTO
    {
        public string Name { get; set; }
        public string ClientType { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public long? ClientId { get; set; }
        public long Id { get; set; }


    }
}
