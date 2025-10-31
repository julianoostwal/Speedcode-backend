using speedCode.Core.rooms.enums;

namespace speedCode.Room.dtos;

public class UpdateRoomDTO
{
    public required string RoomName { get; set; }
    public required bool IsPrivate { get; set; }
    public required Level RoomLevel { get; set; }
    public required List<Guid> exerciseId { get; set; }
    public required List<Guid> profileId { get; set; }
}