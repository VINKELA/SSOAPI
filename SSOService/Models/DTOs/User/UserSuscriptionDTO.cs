using System;

namespace SSOService.Models.DTOs.User
{
    public class UserSuscriptionDTO
    {
        public string SuscriptionName { get; set; }
        public Guid ClientId { get; set; }
        public Guid SuscriptionId { get; set; }
    }

}
