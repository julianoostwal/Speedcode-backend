using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model;

[Table("testCode")]
public class TestCode
{
    [Key] public Guid Id { get; set; }


    [Required]
    [Column("expected_result", TypeName = "text[]")]
    public string[] ExpectedResult { get; set; }

    public Profiles author { get; set; }

    [Required] public DateTime CreatedAt { get; set; }
}