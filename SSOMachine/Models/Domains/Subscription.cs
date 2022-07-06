using SSOService.Models.Enums;

namespace SSOMachine.Models.Domains
{
    public class Subscription
    {
        public string Name { get; set; }
        public ClientType ClientType { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
    }
}
