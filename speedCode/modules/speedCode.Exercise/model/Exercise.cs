using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using speedCode.Core.rooms.enums;

namespace speedCode.exercise.model
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
        public bool isRoomexercise { get; set; }
        
        [Required]
        public bool isPrivate { get; set; }

        [Required]
        public int points { get; set; }
        
        [Required]
        public Level Level { get; set; }
        
        public virtual ICollection<ExerciseTest> ExerciseTests { get; set; } = new List<ExerciseTest>();

    }
}