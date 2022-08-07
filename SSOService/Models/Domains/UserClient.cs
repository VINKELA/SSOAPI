using SSOMachine.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    //These refers to the client users belong to
    public class UserClient : Base
    {
        public UserClient()
        {
            Code = Guid.NewGuid().ToString();
        }
        public Guid UserId { get; set; }
        public Guid ClientId { get; set; }
        public string Code { get; set; }
    }
}
