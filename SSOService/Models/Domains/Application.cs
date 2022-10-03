using SSOService.Models.Enums;
using System;

namespace SSOService.Models.Domains
{
    //application refers to my apps that uses this sso application
    public class Application : EntityTracking
    {
        public long ApplicationId { get; set; }
        public string Name { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string URL { get; set; }
        public long ClientId { get; set; }
        public Client Client { get; set; }
    }
}
