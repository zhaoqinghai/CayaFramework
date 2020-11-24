using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.Core.Entity
{
    /// <summary>
    /// 记录操作（相关）
    /// </summary>
    public interface IAudit<out T> where T : struct
    {
        T AuditUser { get; }
    }
}
