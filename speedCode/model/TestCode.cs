using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model
{
    [Table("testCode")]
    public class TestCode
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("testCode", TypeName = "text")]
        public string testCode { get; set; }

        [Required]
        public virtual Guid authorid { get; set; }
        [ForeignKey("authorid")]
        public virtual Profiles author { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}