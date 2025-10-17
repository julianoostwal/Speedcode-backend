using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using speedCode.Model;

namespace speedCode.Excise.model
{
    [Table("exercise_tests")]
    public class ExerciseTest
    {
        [Key] public Guid id { get; set; } = Guid.NewGuid();

        [Required] public virtual Guid testId { get; set; }
        [ForeignKey("testId")] public virtual TestCode test { get; set; }

        [Required] public virtual Guid exerciseId { get; set; }
        [ForeignKey("exerciseId")] public virtual Exercise exercise { get; set; }
    }
}