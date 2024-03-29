﻿using System;

namespace SSOService.Models.DTOs.Application
{
    public class GetApplicationAuthentificationDTO
    {
        public string ClientSecret { get; set; }
        public string ClientCode { get; set; }
        public string Client { get; set; }
        public string Server { get; set; }
        public bool Status { get; set; }
        public long ClientApplicationId { get; set; }
        public long ServerApplicationId { get; set; }
    }
}
