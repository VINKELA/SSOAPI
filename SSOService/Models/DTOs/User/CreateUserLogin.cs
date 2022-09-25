using SSOService.Models.Enums;
using System;

namespace SSOService.Models.DTOs.User
{
    public class CreateUserLogin
    {
        public Guid UserId { get; set; }
        public LoginProvider LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProdviderDisplayName { get; set; }
    }
}
