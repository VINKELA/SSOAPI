﻿using Microsoft.AspNetCore.Http;
using SSOService.Models.Enums;

namespace SSOService.Models.DTOs
{
    public class UpdateClientDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public ClientType ClientType { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ContactPersonPhoneNumber { get; set; }
        public IFormFile Image { get; set; }
        public string Motto { get; set; }
        public string ParentClientId { get; set; }
    }
}
