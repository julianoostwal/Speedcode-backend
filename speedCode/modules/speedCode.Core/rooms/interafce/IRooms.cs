using speedCode.Core.rooms.enums;

namespace speedCode.Core.rooms;

public interface IRoom
{
    Guid Id { get; set; }
    string RoomName { get; set; }
    Level RoomLevel { get; set; }
    bool IsPrivate { get; set; }
    DateTime CreatedAt { get; set; }
    Guid ChatId { get; set; }
}
