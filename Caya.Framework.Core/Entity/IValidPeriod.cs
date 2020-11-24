using System;
using System.Collections.Generic;
using System.Text;

namespace Caya.Framework.Core.Entity
{
    /// <summary>
    /// 记录的时效
    /// </summary>
    public interface IValidPeriod
    {
        public DateTimeOffset StartDateTime { get; set; }

        public DateTimeOffset EndDateTime{ get; set; }
    }
}
