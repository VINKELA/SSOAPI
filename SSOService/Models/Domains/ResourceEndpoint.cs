using SSOService.Models.Enums;
using System;

namespace SSOService.Models.Domains
{
    public class ResourceEndpoint : EntityTracking
    {
        public long ResourceEndpointId { get; set; }
        public long ResourceId { get; set; }
        public Resource Resource { get; set; }
        public Method Method { get; set; }
        public string Endpoint { get; set; }
    }
}
