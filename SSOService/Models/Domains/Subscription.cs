using SSOService.Models.Enums;
using System;

namespace SSOService.Models.Domains
{
    // subscription refers to a combination of services
    public class Subscription : EntityTracking
    {
        public string Name { get; set; }
        public ClientType ClientType { get; set; }
        public decimal AmountInNaira { get; set; }
        public Interval Interval { get; set; }
        public Guid ClientId { get; set; }
    }
}
