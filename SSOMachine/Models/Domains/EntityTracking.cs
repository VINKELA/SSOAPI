using System;

namespace SSOMachine.Models.Domains
{
    public class EntityTracking : Base
    {
        public string TrackingId { get; set; }
        public string ConcurrencyStamp { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Modified { get; set; }
        public string LastModifiedBy { get; set; }


    }
}
