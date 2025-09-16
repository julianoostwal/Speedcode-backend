using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model
{
    [Table("exercise_tests")]
    public class ExerciseTest
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public TestCode TestId { get; set; }

        [Required]
        public Exercise Exercise { get; set; }
    }
}
