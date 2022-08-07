using SSOService.Models.DTOs.Client;
using SSOService.Models.DTOs.User;
using System.Collections.Generic;

namespace SSOService.Models.DTOs.Auth
{
    public class UserTokenDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public List<UserRoleDTO> Roles { get; set; }
        public List<UserClientDTO> Clients { get; set; }



    }
}
