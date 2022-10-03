using SSOService.Models.Enums;
using System;

namespace SSOService.Models.DTOs
{
    public class GetClientDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public ClientType ClientType { get; set; }
        public string ContactPersonEmail { get; set; }
        public string LogoUrl { get; set; }
        public string Motto { get; set; }
        public string ParentClient { get; set; }
        public string ParentClientName { get; set; }
        public string ClientTypeName { get; set; }
        public bool IsActive { get; set; }
        public long Id { get; set; }
        public byte[] Logo { get; set; }

    }
}
