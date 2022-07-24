using System;

namespace SSOMachine.Models.Domains
{
    public class Role : EntityTracking
    {
        public string Name { get; set; }
        public Guid ClientId { get; set; }
    }
}
