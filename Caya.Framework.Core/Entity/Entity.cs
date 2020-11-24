using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Caya.Framework.Core.Entity
{
    public class Entity<T>
    {
        [Key]
        public virtual T Id { get; set; }
    }
}
