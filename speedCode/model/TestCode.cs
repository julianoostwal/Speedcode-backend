using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model
{
    [Table("testCode")]
    public class TestCode
    {
        [Key] public Guid id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("expected_result", TypeName = "text[]")]
        public string[] ExpectedResult { get; set; }

        [Required] public virtual Guid authorId { get; set; }
        [ForeignKey("authorId")] public virtual Profiles author { get; set; }

        public enum TestType
        {
            Regex,
            Exactly,
            Contains,
        }

        [Column("type")] [Required] public TestType Type { get; set; }

        [Required] public DateTime CreatedAt { get; set; }

        public virtual ICollection<ExerciseTest> ExerciseTests { get; set; } = new List<ExerciseTest>();
    }
}