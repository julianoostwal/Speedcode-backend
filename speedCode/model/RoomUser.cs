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
        [ForeignKey("Room")]
        public Rooms RoomId { get; set; }

        [Required]
        [ForeignKey("Profile")]
        public Profiles ProfileId { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    }
}