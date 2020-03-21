using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Caya.Framework.Core
{
    public class Entity<T>
    {
        [Key]
        public T Id { get; set; }
    }
}
