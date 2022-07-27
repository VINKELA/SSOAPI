using System;

namespace SSOMachine.Models.Domains
{
    public class EntityTracking : Base
    {
        protected EntityTracking()
        {
            ConcurrencyStamp = Guid.NewGuid();
        }
        public Guid ConcurrencyStamp { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Modified { get; set; }
        public string LastModifiedBy { get; set; }
    }
}
