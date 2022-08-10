using System;

namespace SSOService.Models.DTOs.Application
{
    public class GetRoleDTO
    {
        public string Name { get; set; }
        public Guid ClientId { get; set; }
        public string Code { get; set; }
        public Guid Id { get; set; }

    }
}
