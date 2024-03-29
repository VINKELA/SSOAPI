﻿using SSOService.Models.Enums;
using System;

namespace SSOService.Models.Domains
{
    //This is the devices used by a user to access the system
    public class UserDevice : Base
    {
        public long UserDeviceId { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public DeviceType DeviceType { get; set; }
        public string DeviceName { get; set; }
        public bool AcceptsNotification { get; set; }

    }
}
