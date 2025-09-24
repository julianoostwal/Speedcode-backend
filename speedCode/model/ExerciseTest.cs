using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model
{
    [Table("exercise_tests")]
    public class ExerciseTest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public virtual Guid TestId { get; set; }
        [ForeignKey("TestId")]
        public virtual TestCode Test { get; set; }

        [Required]
        public virtual Guid ExerciseId { get; set; }
        [ForeignKey("ExerciseId")]
        public virtual Exercise Exercise { get; set; }
    }
}
