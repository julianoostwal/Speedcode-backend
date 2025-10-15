using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace speedCode.Model
{
    [Table("profiles")]
    public class Profiles
    {
        [Key] [Column(TypeName = "uuid")] public Guid id { get; set; } = Guid.NewGuid();

        [Column("avatar_url", TypeName = "text")]
        public string? avatarUrl { get; set; }

        [Required] public DateTime createdAt { get; set; }

        [Column("nick_name", TypeName = "varchar(50)")]
        public string? nickName { get; set; }

        public enum Rol
        {
            user,
            admin
        }

        public Rol role { get; set; }
    }
}