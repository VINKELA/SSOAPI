using Microsoft.AspNetCore.Http;

namespace SSOService.Models.DTOs.User
{
    public class CreateUserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Confirmation { get; set; }
        public IFormFile File { get; set; }
    }
}
