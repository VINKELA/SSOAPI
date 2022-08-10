using SSOService.Models.Enums;
using System;

namespace SSOService.Models.Domains
{
    // A business that has users this sso, this is our customers
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
        public Guid? ParentClient { get; set; }


    }
}
