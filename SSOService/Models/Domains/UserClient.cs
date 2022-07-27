using SSOMachine.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    public class UserClient : Base
    {
        public Guid UserId { get; set; }
        public Guid ClientId { get; set; }
    }
}
