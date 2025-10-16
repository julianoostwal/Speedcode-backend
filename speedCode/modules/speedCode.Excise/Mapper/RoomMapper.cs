
using speedCode.Core.rooms.dto;
using speedCode.Core.rooms.enums;
using speedCode.Model;

namespace speedCode.Core.rooms.Mapper;

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