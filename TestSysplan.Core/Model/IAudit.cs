using System;

namespace TestSysplan.Core.Model
{
    public interface IAudit
    {
        DateTime Created { get; set; }
        
        DateTime? Updated { get; set; }

    }
}
