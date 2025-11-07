using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Room.Model;

[Table("room_exercises")]
public class RoomExercise
{
    [Key]
    public Guid id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid roomId { get; set; }

    [Required]
    public Guid exerciseId { get; set; }
}