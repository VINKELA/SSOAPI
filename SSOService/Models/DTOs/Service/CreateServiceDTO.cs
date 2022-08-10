using SSOService.Models.Enums;
using System;

namespace SSOService.Models.DTOs.Service
{
    public class CreateServiceDTO
    {
        public string Name { get; set; }
        public Entity ServiceType { get; set; }
        public Guid ClientId { get; set; }

    }
}
