using System;

namespace SSOService.Models.DTOs.Entity
{
    public class GetEntityDTO
    {
        public string Name { get; set; }
        public string UniqueName { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid Id { get; set; }


    }
}
