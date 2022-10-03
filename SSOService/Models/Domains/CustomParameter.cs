using SSOService.Models.Enums;
using System;

namespace SSOService.Models.Domains
{
    public class CustomParameter : EntityTracking
    {
        public long CustomParameterId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public CustomParameterType CustomParameterType { get; set; }
        public long ApplicationResourceId { get; set; }
        public Resource Resource { get; set; } 
    }
}
