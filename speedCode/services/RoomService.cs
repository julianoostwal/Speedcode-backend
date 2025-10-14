using speedCode.Data;
using speedCode.dto.room;
using speedCode.Mapper;
using speedCode.Model;

namespace speedCode.services;

public class RoomService
{
    private readonly AppDbContext _db;

    public RoomService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Rooms> CreateRoom(CreateRoomDTO roomRequest)
    {
        Rooms room = RoomMapper.FromCreateRoomDto(roomRequest);
        _db.Rooms.Add(room);
        await _db.SaveChangesAsync();
        return room;
    }

    public async Task<RoomUser> JoinRoom(Guid profielId, Guid roomId)
    {
        RoomUser roomUser = new RoomUser
        {
            roomId = roomId,
            profileId = profielId
        };
        _db.RoomUser.Add(roomUser);
        await _db.SaveChangesAsync();

        return roomUser;
    }
}