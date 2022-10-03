using SSOService.Models.Enums;
using System;

namespace SSOService.Models.DTOs.Service
{
    public class CreateResourceDTO
    {
        public string Name { get; set; }
        public long ResourceTypeId { get; set; }
        public long ApplicationId { get; set; }
    }
}
