using System;

namespace SSOService.Models.DTOs.ServiceType
{
    public class CreateResourceTypeDTO
    {
        public string Name { get; set; }
        public Guid ApplicationId { get; set; }

    }
}
