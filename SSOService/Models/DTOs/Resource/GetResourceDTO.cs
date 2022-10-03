using System;

namespace SSOService.Models.DTOs.Service
{
    public class GetResourceDTO
    {
        public string Name { get; set; }
        public string ServiceType { get; set; }
        public long Id { get; set; }
        public long ClientId { get; set; }

    }
}
