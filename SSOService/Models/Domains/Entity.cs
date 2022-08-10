using SSOMachine.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    public class Entity : EntityTracking
    {
        public Entity()
        {
            Code = Guid.NewGuid().ToString();
        }
        public string Name { get; set; }
        public string Code { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
