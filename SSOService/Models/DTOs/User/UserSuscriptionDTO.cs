using System;

namespace SSOService.Models.DTOs.User
{
    public class UserSuscriptionDTO
    {
        public string SuscriptionName { get; set; }
        public long ClientId { get; set; }
        public long SuscriptionId { get; set; }
    }

}
