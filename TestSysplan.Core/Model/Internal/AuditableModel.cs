using System;
using System.Text.Json.Serialization;

namespace TestSysplan.Core.Model
{
    public abstract class AuditableModel : ModelBase, IAudit
    {
        [JsonIgnore]
        public DateTime Created { get; set; }

        [JsonIgnore]
        public DateTime? Updated { get; set; }
    }
}
