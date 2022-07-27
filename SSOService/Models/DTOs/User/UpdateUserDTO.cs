using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SSOService.Models.DTOs.User
{
    public class UpdateUserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public IFormFile File { get; set; }
        public List<string> ClientIds { get; set; }
    }
}
