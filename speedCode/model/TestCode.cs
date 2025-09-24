using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model;

[Table("testCode")]
public class TestCode
{
    [Key]
    [Column(TypeName = "uuid")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [Column("testCode", TypeName = "text")]
    public string testCode { get; set; }

    public virtual string authorid { get; set; }
    [ForeignKey("authorid")]
    public virtual Profiles author { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
}