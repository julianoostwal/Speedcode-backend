using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model
{
    [Table("room_users")]
    public class RoomUser
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();

        [Required]
        public virtual Guid roomId { get; set; }
        [ForeignKey("roomId")]
        public virtual Rooms room { get; set; }

        [Required]
        public virtual Guid profileId { get; set; }
        [ForeignKey("profileId")]
        public virtual Profiles profile { get; set; }

        [Required]
        public DateTime createdAt { get; set; }

    }
}