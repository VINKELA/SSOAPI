using System;

namespace SSOService.Models.Domains
{
    public class ResourceType : EntityTracking
    {
        public string Name { get; set; }
        public Guid ClientId { get; set; }
    }
}
