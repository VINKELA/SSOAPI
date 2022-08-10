using System;

namespace SSOService.Models.DTOs.Entity
{
    public class CreateEntityDTO
    {
        public string Name { get; set; }
        public Guid ApplicationId { get; set; }

    }
}
