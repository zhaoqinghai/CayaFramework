using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caya.Framework.Core.Entity
{
    public interface IRowVersion
    {
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
