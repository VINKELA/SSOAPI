using System;

namespace SSOService.Models.DTOs.ReSourceType
{
    public class CreateResourceTypeDTO
    {
        public string Name { get; set; }
        public Guid ApplicationId { get; set; }

    }
}
