using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model;

[Table("testCode")]
public class TestCode
{
    [Key] public Guid Id { get; set; }

    [Required]
    [Column("testCode", TypeName = "text")]
    public string testCode { get; set; }

    public Profiles author { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
}