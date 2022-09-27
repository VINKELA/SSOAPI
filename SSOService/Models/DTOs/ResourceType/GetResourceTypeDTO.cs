﻿using System;

namespace SSOService.Models.DTOs.ReSourceType
{
    public class GetResourceTypeDTO
    {
        public string Name { get; set; }
        public string ResourceType { get; set; }
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }

    }
}
