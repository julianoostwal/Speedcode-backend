using speedCode.Model;

namespace speedCode.dto.room;

public class CreateRoomDTO
{
    public string RoomName { get; set; }

    public Rooms.Level RoomLevel { get; set; }

    public bool IsPrivate { get; set; }
}