using SSOMachine.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    public class RefreshToken : Base
    {

        public Guid UserId { get; set; }
        public Guid TokenId { get; set; }
    }
}
