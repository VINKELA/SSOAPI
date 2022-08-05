using SSOService.Models.DTOs.User;
using System.Collections.Generic;
using System.Security.Claims;

namespace SSOService.Models.DTOs.Auth
{
    public class UserDTO : GetUserDTO
    {
        public List<Claim> Claims { get; set; }
    }
}
