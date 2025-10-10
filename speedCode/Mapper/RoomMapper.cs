using speedCode.dto.room;
using speedCode.Model;

namespace speedCode.Mapper;

public class RoomMapper
{
    public static Rooms FromCreateRoomDto(CreateRoomDTO request)
    {
        return new Rooms
        {
            roomName = request.RoomName,
            roomLevel = request.RoomLevel,
            isPrivate = request.IsPrivate
        };
    }
}