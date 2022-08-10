using SSOService.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    public class ServiceType : EntityTracking
    {
        public string Name { get; set; }
        public Guid ClientId { get; set; }
    }
}
