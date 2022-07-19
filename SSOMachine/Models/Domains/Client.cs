using SSOService.Models.Enums;

namespace SSOMachine.Models.Domains
{
    public class Client : EntityTracking
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public ClientType ClientType { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ContactPersonPhoneNumber { get; set; }
        public string LogoUrl { get; set; }
        public string Motto { get; set; }
        public long? ParentClient { get; set; }

    }
}
