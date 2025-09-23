using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace speedCode.Model
{
    [Table("profiles")]
<<<<<<< HEAD
    [Index(nameof(NickName), IsUnique = true)]
=======
>>>>>>> main
    public class Profiles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Column(TypeName = "text")]
        public string? AvatarUrl { get; set; }

<<<<<<< HEAD
        [Required]
        public DateTime CreatedAt { get; set; }
        
=======
        [Column("avatar_url", TypeName = "text")]
        public string? AvatarUrl { get; set; }

        [Required] public DateTime CreatedAt { get; set; }

>>>>>>> main
        [Column("nick_name", TypeName = "varchar(50)")]
        public string? NickName { get; set; }
    }
}