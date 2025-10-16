using speedCode.Core.rooms.dto;
using speedCode.Core.rooms.Mapper;
using speedCode.Room;
using speedCode.Room.Model;

namespace speedCode.Excise.service;

public class RoomService
{
    private readonly RoomsDb _RoomDb;

    public RoomService(RoomsDb db)
    {
        _RoomDb = db;
    }

    public async Task<Rooms> CreateRoom(CreateRoomDTO roomRequest)
    {
        Rooms room = RoomMapper.FromCreateRoomDto(roomRequest);
        _RoomDb.rooms.Add(room);
        await _RoomDb.SaveChangesAsync();
        return room;
    }
}