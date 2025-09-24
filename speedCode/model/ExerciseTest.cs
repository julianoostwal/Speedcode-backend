using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model
{
    [Table("exercise_tests")]
    public class ExerciseTest
    {
        [Key]
        [Column(TypeName = "uuid")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public virtual string TestId { get; set; }
        [ForeignKey("TestId")]
        public virtual TestCode Test { get; set; }

        [Required]
        public virtual string ExerciseId { get; set; }
        [ForeignKey("ExerciseId")]
        public virtual Exercise Exercise { get; set; }
    }
}
