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

        [NotMapped]
        public string Judge0Stdin
        {
            get
            {
                if (TestExpectIOs == null || !TestExpectIOs.Any())
                    return string.Empty;
                return string.Join("\n", TestExpectIOs
                    .Where(io => !string.IsNullOrWhiteSpace(io.input))
                    .Select(io => io.input.Trim()));
            }
        }
    }
}