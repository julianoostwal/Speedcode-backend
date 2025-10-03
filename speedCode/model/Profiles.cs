using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace speedCode.Model
{
    [Table("profiles")]
    public class Profiles
    {
        [Key]
        [Column(TypeName = "uuid")]
        public Guid id { get; set; }

        [Column("avatar_url", TypeName = "text")]
        public string? avatarUrl { get; set; }

        [Required] public DateTime createdAt { get; set; }

        [Column("nick_name", TypeName = "varchar(50)")]
        public string? nickName { get; set; }

        public enum Rol { Admin, User }

        public Rol profileRol { get; set; }
    }
}