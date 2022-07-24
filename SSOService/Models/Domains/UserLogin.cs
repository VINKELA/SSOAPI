using SSOService.Models.Enums;
using System;

namespace SSOMachine.Models.Domains
{
    public class UserLogin : Base
    {
        public Guid UserId { get; set; }
        public LoginProvider LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProdviderDisplayName { get; set; }
    }
}
