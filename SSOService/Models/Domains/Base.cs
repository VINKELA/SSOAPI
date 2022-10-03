using System;

namespace SSOService.Models.Domains
{
    public class Base
    {
        protected Base()
        {
            IsActive = true;
            IsDeleted = false;
            CreatedOn = DateTime.Now;

        }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }

    }


}
