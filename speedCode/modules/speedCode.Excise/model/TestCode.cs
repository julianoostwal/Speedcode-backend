using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using speedCode.Core.judge0.dto;

namespace speedCode.Excise.model
{
    [Table("testCode")]
    public class TestCode
    {
        [Key] public Guid id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("authorId")]
        public Guid AuthorId { get; set; }

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