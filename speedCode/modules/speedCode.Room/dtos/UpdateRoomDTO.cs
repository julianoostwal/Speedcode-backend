namespace speedCode.Room.dtos;

public class UpdateRoomDTO
{
    public required Guid roomId { get; set; }
    public required List<Guid> exerciseId { get; set; }
    public required List<Guid> profileId { get; set; }
}