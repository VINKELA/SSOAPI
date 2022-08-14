using SSOService.Models.Enums;
using System;

namespace SSOService.Models.Domains
{
    public class CustomParameter : Base
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public CustomParameterType CustomParameterType { get; set; }
        public Guid ServiceId { get; set; }
    }
}
