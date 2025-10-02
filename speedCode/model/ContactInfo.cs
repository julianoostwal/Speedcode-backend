using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace speedCode.Model
{
    [Table("contact_info")]
    public class ContactInfo
    {
        [Key]
        [Column(TypeName = "uuid")]
        public Guid id { get; set; }

        public string city { get; set; }

        public string address { get; set; }

        public string postalCode { get; set; }

        public string emailAddress { get; set; }
    }
}