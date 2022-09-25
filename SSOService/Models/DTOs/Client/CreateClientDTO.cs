using Microsoft.AspNetCore.Http;
using SSOService.Models.Enums;

namespace SSOService.Models.DTOs
{
    public class CreateClientDTO
    {
        public string Name { get; set; }
        public ClientType ClientType { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ContactPersonFirstName { get; set; }
        public string ContactPersonLastName { get; set; }
        public IFormFile Logo { get; set; }
    }
}
