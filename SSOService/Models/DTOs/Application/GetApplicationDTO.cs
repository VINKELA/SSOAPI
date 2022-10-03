using System;

namespace SSOService.Models.DTOs.Application
{
    public class GetApplicationDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ApplicationType { get; set; }
        public string URL { get; set; }
        public string ServiceType { get; set; }
        public long? ClientId { get; set; }

    }
}
