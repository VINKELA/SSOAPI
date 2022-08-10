using SSOService.Models.Enums;
using System;

namespace SSOService.Models.Domains
{
    public class ApplicationServices
    {
        public Guid ApplicationId { get; set; }
        public Guid ServiceId { get; set; }
        public Method Method { get; set; }
        public string Endpoint { get; set; }

    }
}
