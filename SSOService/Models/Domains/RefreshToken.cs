using SSOService.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    public class RefreshToken : Base
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
    }
}
