using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model;

[Table("testCode")]
public class TestCode
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [Column(TypeName = "text")]
    public string Code { get; set; }

    [Required]
    [ForeignKey("Author")]
    public Profiles Author { get; set; }

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}