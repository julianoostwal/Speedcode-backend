
using speedCode.Core.rooms.dto;


namespace speedCode.Core.rooms.Mapper;

public class RoomMapper
{
    public static Rooms FromCreateRoomDto(CreateRoomDTO request)
    {
        return new Rooms.Rooms
        {
            roomName = request.RoomName,
            roomLevel = request.RoomLevel,
            isPrivate = request.IsPrivate
        };
    }
}