using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model
{
    [Table("exercises")]
    public class Exercise
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public bool isRoomExercise { get; set; }

        [Required]
        public int points { get; set; }
    }
}