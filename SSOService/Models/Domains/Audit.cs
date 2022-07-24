using SSOMachine.Models.Domains;
using SSOService.Models.Enums;
using System;

namespace SSOService.Models.Domains
{
    public class Audit : EntityTracking
    {
        public Guid UserId { get; set; }
        public AuditType Type { get; set; }
        public string TableName { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string AffectedColumns { get; set; }
        public string PrimaryKey { get; set; }
    }
}
