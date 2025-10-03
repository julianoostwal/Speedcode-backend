using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using speedCode.Model;

namespace speedCode.Model
{
    [Table("nav_items")]
    public class NavItem
    {
        [Key]
        [Column(TypeName = "uuid")]
        public Guid id { get; set; }

        public string label { get; set; }

        public string href { get; set; }

        public bool isDropdown { get; set; }

        public virtual Guid dropdownItemsId { get; set; }
        [ForeignKey("dropdownItemsId")]
        public virtual DropdownItems dropdownItems { get; set; }
    }
}