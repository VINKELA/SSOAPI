using System;

namespace SSOService.Models.Domains
{
    public class ServiceType : EntityTracking
    {
        public long ServiceTypeId { get; set; }    
        public string Name { get; set; }
        public long ApplicationId { get; set; }
        public Application Application { get; set; }
    }
}
