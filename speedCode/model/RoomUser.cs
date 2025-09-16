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

        [Key]
        [Required]
        [ForeignKey("Room")]
        public Rooms RoomId { get; set; }

        [Key]
        [Required]
        [ForeignKey("Profile")]
        public Profiles ProfileId { get; set; }

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    }
}