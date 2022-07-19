using SSOMachine.Models.Enums;

namespace SSOMachine.Models.Domains
{
    public class UserDevice : Base
    {
        public long UserId { get; set; }
        public DeviceType DeviceType { get; set; }
        public string DeviceName { get; set; }
        public bool AcceptsNotification { get; set; }

    }
}
