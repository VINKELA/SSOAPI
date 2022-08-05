using SSOMachine.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    public class Entity : EntityTracking
    {
        public string Name { get; set; }
        public string UniqueName { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
