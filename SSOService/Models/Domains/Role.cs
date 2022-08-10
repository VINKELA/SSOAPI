﻿using System;

namespace SSOService.Models.Domains
{
    //Roles created by clients for their users
    public class Role : EntityTracking
    {
        public string Name { get; set; }
        public Guid ClientId { get; set; }
    }
}
