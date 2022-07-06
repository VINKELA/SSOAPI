using SSOService.Models.Enums;

namespace SSOMachine.Models.Domains
{
    public class UserLogin : Base
    {
        public long UserId { get; set; }
        public LoginProvider LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProdviderDisplayName { get; set; }
    }
}
