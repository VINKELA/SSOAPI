using System;

namespace SSOMachine.Models.Domains
{
    public class Base
    {
        protected Base()
        {
            Id = new Guid();
            IsActive = true;
            IsDeleted = false;
        }
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }


}
