using SSOService.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    public class RefreshToken : Base
    {
        public long RefreshTokenId { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
    }
}
