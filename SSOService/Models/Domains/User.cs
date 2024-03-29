﻿using System;

namespace SSOService.Models.Domains
{
    // This are users of the services
    public class User : EntityTracking
    {
        public long UserId { get; set; }    
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
        public string FilePath { get; set; }
        public long? ClientId { get; set; }
        public Client Client { get; set; }

    }
}
