using System;
using speedCode.Core.interfaces;

namespace speedCode.Room.dtos;

public class RoomExerciseDto
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid ExerciseId { get; set; }
    public ExerciseDto Exercise { get; set; }
}