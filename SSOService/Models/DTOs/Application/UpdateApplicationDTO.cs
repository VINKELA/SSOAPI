using SSOMachine.Models.Enums;

namespace SSOService.Models.DTOs.Application
{
    public class UpdateApplicationDTO
    {
        public string Name { get; set; }
        public ApplicationType? ApplicationType { get; set; }
        public string URL { get; set; }
        public ServiceType? ServiceType { get; set; }
    }
}
