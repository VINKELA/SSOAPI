using SSOService.Models.Enums;
using System;
using System.Collections.Generic;

namespace SSOService.Models.DTOs.Audit
{
    public class AuditEntry
    {
        public Guid UserId { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public AuditType AuditType { get; set; }
        public List<string> ChangedColumns { get; } = new List<string>();

    }
}
