using System;

namespace SSOService.Models.DTOs.Application
{
    public class GetApplicationDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ApplicationType { get; set; }
        public string URL { get; set; }
        public string ServiceType { get; set; }
        public Guid? ClientId { get; set; }

    }
}
