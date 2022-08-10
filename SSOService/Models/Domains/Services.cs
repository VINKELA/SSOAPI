using SSOService.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    //services refers to values provided by applications
    public class Service : EntityTracking
    {
        public string Name { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid ClientId { get; set; }
    }
}
