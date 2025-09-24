using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model
{
    [Table("exercises")]
    public class Exercise
    {
        [Key]
        [Column(TypeName = "uuid")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string title { get; set; }

        public string description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public bool isRoomExcise { get; set; }

        [Required]
        public int points { get; set; }
    }
}