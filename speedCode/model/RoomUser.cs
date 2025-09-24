using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace speedCode.Model
{
    [Table("room_users")]
    public class RoomUser
    {
        [Key]
        [Column(TypeName = "uuid")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public virtual string RoomId { get; set; }
        [ForeignKey("RoomId")]
        public virtual Rooms Room { get; set; }

        [Required]
        public virtual string Profileid { get; set; }
        [ForeignKey("Profileid")]
        public virtual Profiles Profile { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        
    }
}