using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace speedCode.Model
{
    [Table("footer_section_links")]
    public class FooterSectionLinks
    {
        [Key]
        [Column(TypeName = "uuid")]
        public Guid id { get; set; }

        public string label { get; set; }

        public string href { get; set; }

        public bool isExternal { get; set; }
    }
}