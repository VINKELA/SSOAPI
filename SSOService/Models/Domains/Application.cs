using SSOMachine.Models.Enums;
using System;

namespace SSOMachine.Models.Domains
{
    //application refers to my apps that uses this sso application
    public class Application : EntityTracking
    {
        public string Name { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string URL { get; set; }
        public ServiceType? ServiceType { get; set; }
        public Guid? ClientId { get; set; }
    }
}
