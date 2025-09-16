using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model
{
    [Table("exercises")]
    public class Exercise
    {
        [Key]
        public Guid Id { get; set; }
        
    }
}