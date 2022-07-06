using SSOMachine.Models.Enums;

namespace SSOMachine.Models.Domains
{
    public class Application : EntityTracking
    {
        public string Name { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string URL { get; set; }
        public ServiceType? ServiceType { get; set; }
    }
}
