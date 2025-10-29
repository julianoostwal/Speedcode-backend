using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using speedCode.Core.rooms.enums;
using speedCode.Excise.model;
using speedCode.Profile.model;


namespace speedCode.Room.Model;

[Table("rooms")]
public class Rooms
{
    [Key] public Guid id { get; set; } = Guid.NewGuid();

    [Required] [Column(TypeName = "text")] public string roomName { get; set; }

    public Level roomLevel { get; set; }

    [Required]
    [Column(TypeName = "boolean")]
    public bool isPrivate { get; set; }

    [Required] public DateTime createdAt { get; set; }

    public Guid chatId { get; set; } = Guid.Empty;

    public virtual ICollection<RoomExercise> RoomExercises { get; set; } = new List<RoomExercise>();
    public virtual ICollection<RoomUser> RoomUsers { get; set; } = new List<RoomUser>();
}