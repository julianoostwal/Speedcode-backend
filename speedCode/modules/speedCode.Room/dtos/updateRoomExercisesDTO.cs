namespace speedCode.Room.dtos;

public class updateRoomExercisesDTO
{
    public required Guid roomId { get; set; }
    public required List<Guid> exerciseId { get; set; }
}