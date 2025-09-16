using System;
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
        public Guid Id { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "text")]
        public string NickName { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string AvatarUrl { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Password { get; set; }

    }
}