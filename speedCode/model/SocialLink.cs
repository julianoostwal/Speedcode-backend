using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace speedCode.Model
{
    [Table("social_links")]
    public class SocialLink
    {
        [Key]
        [Column(TypeName = "uuid")]
        public Guid id { get; set; }

        public string platform { get; set; }

        public string href { get; set; }

        public string icon { get; set; }
    }
}