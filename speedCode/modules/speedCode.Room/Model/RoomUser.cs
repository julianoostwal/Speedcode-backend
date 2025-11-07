using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Room.Model;

[Table("room_users")]
public class RoomUser
{
    [Key] public Guid id { get; set; } = Guid.NewGuid();

    [Required] public Guid roomId { get; set; }

    [Required] public Guid profileId { get; set; }

    [Required] public DateTime createdAt { get; set; }
}