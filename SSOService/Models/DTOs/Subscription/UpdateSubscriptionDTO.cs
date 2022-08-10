using SSOService.Models.Enums;

namespace SSOService.Models.DTOs.Subscription
{
    public class UpdateSubscriptionDTO
    {
        public string Name { get; set; }
        public ClientType ClientType { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
    }
}
