using SSOService.Models.Enums;
using System;

namespace SSOService.Models.Domains
{
    //application refers to my apps that uses this sso application
    public class Application : EntityTracking
    {
        public string Name { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string URL { get; set; }
        public Guid ClientId { get; set; }
    }
}
