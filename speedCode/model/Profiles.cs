using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace speedCode.Model
{
    [Table("profiles")]
    [Index(nameof(Email), IsUnique = true), Index(nameof(NickName), IsUnique = true)]
    public class Profiles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Password { get; set; }

        [Column(TypeName = "text")]
        public string AvatarUrl { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        
        [Column("nick_name", TypeName = "varchar(50)")]
        public string? NickName { get; set; }
    }
}