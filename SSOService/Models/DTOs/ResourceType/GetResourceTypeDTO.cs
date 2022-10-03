using System;

namespace SSOService.Models.DTOs.ReSourceType
{
    public class GetServiceTypeDTO
    {
        public string Name { get; set; }
        public string ResourceType { get; set; }
        public long Id { get; set; }
        public long ApplicationId { get; set; }

    }
}
