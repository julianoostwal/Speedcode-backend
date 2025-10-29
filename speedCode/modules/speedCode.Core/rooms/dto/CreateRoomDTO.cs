using speedCode.Core.rooms.enums;

namespace speedCode.Core.rooms.dto;

public class CreateRoomDTO
{
    public string RoomName { get; set; }

    public Level RoomLevel { get; set; }

    public bool IsPrivate { get; set; }

    public required List<Guid> AllowedUsers { get; set; }
}