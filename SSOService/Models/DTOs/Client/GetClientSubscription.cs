using System;

namespace SSOService.Models.DTOs.Client
{
    public class GetClientSubscription
    {
        public string Client { get; set; }
        public string Subscription { get; set; }
        public bool Status { get; set; }
        public long ClientId { get; set; }
        public long SubcriptionId { get; set; }

    }
}
