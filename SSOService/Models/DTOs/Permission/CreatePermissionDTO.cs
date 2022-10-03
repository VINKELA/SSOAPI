using SSOService.Models.Enums;
using System;

namespace SSOService.Models.DTOs.Permission
{
    public class CreatePermissionDTO
    {
        public PermissionType PermissionType { get; set; }
        public Scope Scope { get; set; }
        public long ResourceId { get; set; }
        public string Name { get; set; }

    }
}
