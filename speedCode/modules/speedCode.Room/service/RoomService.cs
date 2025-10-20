using speedCode.Core.rooms.dto;
using speedCode.Core.rooms.Mapper;
using speedCode.Profile;
using speedCode.Room;
using speedCode.Room.Model;

namespace speedCode.Excise.service;

public class RoomService
{
    private readonly RoomsDb _RoomDb;
    private readonly ProfileDb _profileDb;

    public RoomService(RoomsDb db, ProfileDb profileDb)
    {
        _RoomDb = db;
        _profileDb = profileDb;
    }

    public async Task<Rooms> CreateRoom(CreateRoomDTO roomRequest)
    {
        Rooms room = RoomMapper.FromCreateRoomDto(roomRequest);
        _RoomDb.rooms.Add(room);
        await _RoomDb.SaveChangesAsync();
        return room;
    }

    public List<Guid> GetAllFinishedExcisesByRoomAndUserId(Guid roomId, Guid profileId)
    {
        return (from pe in _profileDb.ProfileExercise
                join re in _RoomDb.RoomExercise
                    on pe.exerciseId equals re.exerciseId
                where pe.profileId == profileId && re.roomId == roomId
                select pe.exerciseId)
            .ToList();
    }
}