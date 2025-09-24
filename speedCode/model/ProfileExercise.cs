using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model
{
    [Table("profile_exercises")]
    public class ProfileExercise
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public virtual Guid ExerciseId { get; set; }
        [ForeignKey("ExerciseId")]
        public virtual Exercise Exercise { get; set; }

        [Required]
        public virtual Guid Profileid { get; set; }
        [ForeignKey("Profileid")]
        public virtual Profiles Profile { get; set; }
    }
}