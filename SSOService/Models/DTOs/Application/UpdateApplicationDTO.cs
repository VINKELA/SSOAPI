using SSOMachine.Models.Enums;
using System;

namespace SSOService.Models.DTOs.Application
{
    public class UpdateApplicationDTO
    {
        public string Name { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string URL { get; set; }
        public ServiceType? ServiceType { get; set; }
        public Guid ClientId { get; set; }
        public Guid ClientSecret { get; set; }
    }
}
