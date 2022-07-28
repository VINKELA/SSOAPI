using SSOMachine.Models.Domains;
using SSOMachine.Models.Enums;
using System;

namespace SSOService.Models.Domains
{
    //services refers to values provided by applications
    public class Service : EntityTracking
    {
        public string Name { get; set; }
        public ServiceType ServiceType { get; set; }
        public Guid ClientId { get; set; }
    }
}
