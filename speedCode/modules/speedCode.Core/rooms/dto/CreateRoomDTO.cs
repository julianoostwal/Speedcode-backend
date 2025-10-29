using speedCode.Core.rooms.enums;

namespace speedCode.Core.rooms.dto;

public class CreateRoomDTO
{
    public string RoomName { get; set; }

    public Level RoomLevel { get; set; }

    public Guid profileId { get; set; }

    public bool IsPrivate { get; set; }
}