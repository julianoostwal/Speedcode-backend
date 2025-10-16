using speedCode.Core.rooms.dto;
using speedCode.Core.rooms.Mapper;
using speedCode.Model;
using speedCode.Profile;

namespace speedCode.Excise.service;

public class RoomService
{
    private readonly ExciseDb _db;
    private readonly ProfileDb _profileDb;

    public RoomService(ExciseDb db)
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
        _profileDb.RoomUser.Add(roomUser);
        await _db.SaveChangesAsync();

        return roomUser;
    }
}