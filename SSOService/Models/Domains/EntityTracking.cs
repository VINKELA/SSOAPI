using System;

namespace SSOService.Models.Domains
{
    public class EntityTracking : Base
    {
        protected EntityTracking()
        {
            ConcurrencyStamp = Guid.NewGuid();
            Code = Guid.NewGuid().ToString();
        }
        public Guid ConcurrencyStamp { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Modified { get; set; }
        public string LastModifiedBy { get; set; }
        public string Code { get; set; }
    }
}
