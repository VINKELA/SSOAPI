using System;

namespace SSOService.Models.Domains
{
    //These refers to the client users belong to
    public class UserClient : Base
    {
        public Guid UserId { get; set; }
        public Guid ClientId { get; set; }
    }
}
