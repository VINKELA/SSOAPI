using SSOMachine.Models.Enums;
using SSOService.Models.Enums;
using System;

namespace SSOService.Models.DTOs.Subscription
{
    public class CreateSubscriptionDTO
    {
        public string Name { get; set; }
        public ClientType ClientType { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public Guid? ClientId { get; set; }

    }
}
