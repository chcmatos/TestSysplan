using System;

namespace TestSysplan.Core.Model
{
    public abstract class AuditableModel : ModelBase, IAudit
    {
        public DateTime Created { get; set; }
        
        public DateTime? Updated { get; set; }
    }
}
