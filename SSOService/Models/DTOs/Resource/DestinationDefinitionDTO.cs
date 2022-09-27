using SSOService.Models.Domains;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SSOService.Models.DTOs.Resource
{
    public class DestinationDefinitionDTO
    {
        internal string EndPointUrl;
        internal string EndPointMethod;
        internal IEnumerable<CustomParameter> CustomHeaders;
    }
}
