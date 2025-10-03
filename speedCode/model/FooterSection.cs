using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using speedCode.Model;

namespace speedCode.Model
{
    [Table("footer_sections")]
    public class FooterSection
    {
        [Key]
        [Column(TypeName = "uuid")]
        public Guid id { get; set; }

        public string title { get; set; }

        public virtual Guid linksId { get; set; }
        [ForeignKey("linksId")]
        public virtual FooterSectionLinks links { get; set; }
    }
}