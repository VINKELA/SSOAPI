using SSOMachine.Models.Enums;
using System;

namespace SSOMachine.Models.Domains
{
    //This is the devices used by a user to access the system
    public class UserDevice : Base
    {
        public Guid UserId { get; set; }
        public DeviceType DeviceType { get; set; }
        public string DeviceName { get; set; }
        public bool AcceptsNotification { get; set; }

    }
}
