namespace speedCode.WsRoom.DTO;

public class FinishARoomExerciseDto
{
    public required Guid ExerciseId { get; set; }
    public required rooms.Room Room { get; set; }
    public required Double memory { get; set; }
    public required TimeSpan time { get; set; }
    public required Guid clientId { get; set; }
}