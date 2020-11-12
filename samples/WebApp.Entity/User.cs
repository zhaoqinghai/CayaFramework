using Caya.Framework.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Entity
{
    [Table("User")]
    public class User : Entity<Guid>
    {
        [Column]
        public string Name { get; set; }

        [Column]
        public int Age { get; set; }
    }
}
