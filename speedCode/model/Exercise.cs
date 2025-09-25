using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model
{
    [Table("exercises")]
    public class Exercise
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();

        public string title { get; set; }

        public string description { get; set; }

        [Required]
        public DateTime createdAt { get; set; }

        [Required]
        public bool isRoomExcise { get; set; }

        [Required]
        public int points { get; set; }
    }
}