using System;

namespace SSOService.Models.DTOs.Application
{
    public class GetRoleDTO
    {
        public string Name { get; set; }
        public long ClientId { get; set; }
        public string Code { get; set; }
        public long Id { get; set; }

    }
}
