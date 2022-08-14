using SSOService.Models.Enums;
using System;

namespace SSOService.Models.Domains
{
    public class ApplicationResource : EntityTracking
    {
        public Guid ApplicationId { get; set; }
        public Guid ResourceId { get; set; }
        public Method Method { get; set; }
        public string Endpoint { get; set; }
    }
}
