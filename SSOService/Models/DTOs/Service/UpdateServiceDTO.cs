using SSOMachine.Models.Enums;

namespace SSOService.Models.DTOs.Service
{
    public class UpdateServiceDTO
    {
        public string Name { get; set; }
        public ServiceType ServiceType { get; set; }
    }
}
