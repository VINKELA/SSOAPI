using System;

namespace SSOService.Models.DTOs.Subscription
{
    public class GetSubscriptionDTO
    {
        public string Name { get; set; }
        public string ClientType { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public Guid? ClientId { get; set; }
        public Guid Id { get; set; }


    }
}
