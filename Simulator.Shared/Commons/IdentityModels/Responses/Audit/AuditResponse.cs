using System;

namespace Simulator.Shared.Commons.IdentityModels.Responses.Audit
{
    public class AuditResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public string OldValues { get; set; } = string.Empty;
        public string NewValues { get; set; } = string.Empty;
        public string AffectedColumns { get; set; } = string.Empty;
        public string PrimaryKey { get; set; } = string.Empty;
    }
}