using System;

namespace SSOService.Models.DTOs.ServiceType
{
    public class CreateServiceTypeDTO
    {
        public string Name { get; set; }
        public Guid ClientId { get; set; }

    }
}
