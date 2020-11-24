using System;

namespace Caya.Framework.Core.Entity
{
    /// <summary>
    /// 记录软删除
    /// </summary>
    public interface ISoftDelete
    {
        public bool IsSoftDelete { get; set; }
    }
}
