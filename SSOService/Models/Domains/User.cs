using System;

namespace SSOMachine.Models.Domains
{
    public class User : EntityTracking
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockOutEnabled { get; set; }
        public DateTime LockoutEnds { get; set; }
        public string UserName { get; set; }
        public int AccessFailedCount { get; set; }
        public Guid? ClientId { get; set; }

    }
}
