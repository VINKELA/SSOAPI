using SSOService.Models.Enums;
using System;

namespace SSOService.Models.DTOs.User
{
    public class CreateUserDevice
    {
        public Guid UserId { get; set; }
        public DeviceType DeviceType { get; set; }
        public string DeviceName { get; set; }
        public bool AcceptsNotification { get; set; }

    }
}
