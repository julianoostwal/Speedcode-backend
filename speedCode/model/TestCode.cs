using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model
{
    [Table("testCode")]
    public class TestCode
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();



        [Required]
        [Column("expected_result", TypeName = "text[]")]
        public string[] ExpectedResult { get; set; }

        [Required]
        public virtual Guid authorId { get; set; }
        [ForeignKey("authorId")]
        public virtual Profiles author { get; set; }

        [Required] public DateTime CreatedAt { get; set; }
    }
}