using SSOService.Models.Enums;
using System;

namespace SSOService.Models.DTOs.Application
{
    public class CreateApplicationDTO
    {
        public string Name { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string URL { get; set; }
        public long ClientId { get; set; }

    }
}
