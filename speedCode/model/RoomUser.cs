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
        public Guid Id { get; set; }

        [Required]
        public Rooms Room { get; set; }

        [Required]
        public Profiles Profile{ get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    }
}