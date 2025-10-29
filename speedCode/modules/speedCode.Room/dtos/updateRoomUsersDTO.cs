namespace speedCode.Room.dtos;

public class updateRoomUsersDTO
{
    public required Guid RoomId { get; set; }
    public required List<Guid> Profilesid { get; set; }
}