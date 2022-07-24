using System;

namespace SSOService.Models.DTOs.User
{
    public class GetUserDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public Guid? ClientId { get; set; }
        public string ClientName { get; set; }
        public bool IsActive { get; set; }
    }
}
