using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using speedCode.dto;

namespace speedCode.Model
{
    [Table("testCode")]
    public class TestCode
    {
        [Key] public Guid id { get; set; } = Guid.NewGuid();
        
        [Required] public Guid authorId { get; set; }
        [ForeignKey("authorId")] public virtual Profiles author { get; set; }

        [Column("type")] [Required] public TestType Type { get; set; }


        public virtual ICollection<ExerciseTest> ExerciseTests { get; set; } = new List<ExerciseTest>();
        public virtual ICollection<TestExpectIO> TestExpectIOs { get; set; } = new List<TestExpectIO>();
        [Required] public DateTime createdAt { get; set; }

    }
}