using SSOService.Models.Domains;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SSOService.Models.DTOs.Resource
{
    public class DestinationDefinitionDTO
    {
        public string EndPointUrl { get; set; }
        public string EndPointMethod { get; set; }
        public IEnumerable<CustomParameter> CustomParameters { get; set; }
    }
}
