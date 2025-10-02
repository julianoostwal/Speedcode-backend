using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace speedCode.Model
{
    [Table("dropdown_items")]
    public class DropdownItems
    {
        [Key]
        [Column(TypeName = "uuid")]
        public Guid id { get; set; }

        public string label { get; set; }

        public string href { get; set; }

        public string description { get; set; }
    }
}