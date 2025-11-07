using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using speedCode.Core.judge0.dto;

namespace speedCode.Exercise.model
{
    [Table("testCode")]
    public class TestCode
    {
        [Key] public Guid id { get; set; } = Guid.NewGuid();

        [Column("type")] [Required] public TestType Type { get; set; }

        [Column("languageId")] [Required] public int LanguageId { get; set; }

        // anti cheat regex 
        [Column("AC_regex")] public string? ACRegex { get; set; }

        [Required] public DateTime createdAt { get; set; }
    }
}