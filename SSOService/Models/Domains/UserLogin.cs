using SSOService.Models.Enums;
using System;

namespace SSOService.Models.Domains
{
    //This is the user logins
    public class UserLogin : Base
    {
        public long UserLoginId { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public LoginProvider LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProdviderDisplayName { get; set; }
    }
}
