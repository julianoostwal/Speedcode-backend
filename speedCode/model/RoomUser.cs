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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public virtual int RoomId { get; set; }
        [ForeignKey("RoomId")]
        public virtual Rooms Room { get; set; }

        [Required]
        public virtual int ProfileId { get; set; }
        [ForeignKey("ProfileId")]
        public virtual Profiles Profile { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        
    }
}