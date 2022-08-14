using System;

namespace SSOService.Models.Domains
{
    //services refers to values provided by applications
    public class Resource : EntityTracking
    {
        public string Name { get; set; }
        public Guid ResourceTypeId { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
