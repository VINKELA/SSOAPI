using System;

namespace SSOService.Models.DTOs.Client
{
    public class GetClientSubscription
    {
        public string Client { get; set; }
        public string Subscription { get; set; }
        public bool Status { get; set; }
        public Guid ClientId { get; set; }
        public Guid SubcriptionId { get; set; }

    }
}
