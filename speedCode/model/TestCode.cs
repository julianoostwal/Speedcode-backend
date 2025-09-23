using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model;

[Table("testCode")]
public class TestCode
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column("testCode", TypeName = "text")]
    public string testCode { get; set; }

    public virtual int authorid { get; set; }
    [ForeignKey("authorid")]
    public virtual Profiles author { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
}