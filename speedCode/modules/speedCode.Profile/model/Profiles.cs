using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using speedCode.Profile;

namespace speedCode.Profile.Model
{
    [Table("profiles")]
    public class Profiles
    {
        [Key] [Column(TypeName = "uuid")] public Guid id { get; set; } = Guid.NewGuid();

        [Column("avatarUrl", TypeName = "text")]
        public string? avatarUrl { get; set; }

        [Required] public DateTime createdAt { get; set; }

        [Column("nickName", TypeName = "varchar(50)")]
        public string? nickName { get; set; }

        public Rol role { get; set; }
    }
}