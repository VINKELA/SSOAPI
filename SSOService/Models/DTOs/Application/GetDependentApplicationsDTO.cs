using System.Collections.Generic;

namespace SSOService.Models.DTOs.Application
{
    public class GetDependentApplicationsDTO
    {
        public string Name { get; set; }
        public string URL { get; set; }
        public int Port { get; set; }
        public int ApplicationType { get; set; }
        public List<ResourceTypeDTO> ResourceTypes{ get; set; }
    }
    public class RequestParameterDTO 
    { 
        public string Name { get; set; }
        public string Value { get; set; }
        public int Type { get; set; }
    }
    public class ResourceEndPointsDTO
    {
        public string URL { get; set; }
        public string Method { get; set; }
        public List<RequestParameterDTO> Parameters { get; set; }
    }
    public class ResourceDTO
    {
        public string Name { get; set; }
        public List<ResourceEndPointsDTO> ResourceEndPoints { get; set; }
    }
    public class ResourceTypeDTO 
    { 
        public string Name { get; set; }
        public List<ResourceDTO> Resources { get; set; }
    }

}
