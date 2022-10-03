using System;

namespace SSOService.Models.Domains
{
    //services refers to values provided by applications
    public class Resource : EntityTracking
    {
        public long ResourceId { get; set; }
        public string Name { get; set; }
        public long ResourceTypeId { get; set; }
        public ServiceType ResourceType { get; set; }  
    }
}
