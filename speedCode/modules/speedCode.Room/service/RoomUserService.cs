using speedCode.Room;
using speedCode.Room.Model;

namespace speedCode.Profile;

public class RoomUserService
{
    private readonly RoomsDb _roomDb;

    public async Task<RoomUser> JoinRoom(Guid profielId, Guid roomId)
    {
        RoomUser roomUser = new RoomUser
        {
            roomId = roomId,
            profileId = profielId
        };
        _roomDb.RoomUser.Add(roomUser);
        await _roomDb.SaveChangesAsync();

        return roomUser;
    }
}