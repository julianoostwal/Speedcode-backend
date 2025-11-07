using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using speedCode.Core.rooms.enums;


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

    [Required] public Guid CreatedById { get; set; }
}