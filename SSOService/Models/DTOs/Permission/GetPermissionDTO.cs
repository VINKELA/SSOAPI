using System;

namespace SSOService.Models.DTOs.Permission
{
    public class GetPermissionDTO
    {
        public string PermissionType { get; set; }
        public string Scope { get; set; }
        public string Name { get; set; }
        public Guid ResourceId { get; set; }
    }
}
