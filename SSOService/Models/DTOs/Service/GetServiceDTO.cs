using System;

namespace SSOService.Models.DTOs.Service
{
    public class GetServiceDTO
    {
        public string Name { get; set; }
        public string ServiceType { get; set; }
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }

    }
}
