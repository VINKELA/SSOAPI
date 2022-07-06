using SSOMachine.Models.Domains;
using SSOMachine.Models.Enums;

namespace SSOService.Models.Domains
{
    public class Services : EntityTracking
    {
        public string Name { get; set; }
        public ServiceType ServiceType { get; set; }
    }
}
