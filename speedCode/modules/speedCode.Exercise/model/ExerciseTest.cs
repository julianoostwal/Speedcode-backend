using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Exercise.model
{
    [Table("exercise_tests")]
    public class ExerciseTest
    {
        [Key] public Guid id { get; set; } = Guid.NewGuid();

        [Required] public Guid testId { get; set; }

        [Required] public Guid exerciseId { get; set; }
    }
}