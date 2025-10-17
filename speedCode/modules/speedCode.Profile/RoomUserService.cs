using speedCode.Profile.model;

namespace speedCode.Profile;

public class RoomUserService
{
    private readonly ProfileDb _profileDb;

    public async Task<RoomUser> JoinRoom(Guid profielId, Guid roomId)
    {
        RoomUser roomUser = new RoomUser
        {
            roomId = roomId,
            profileId = profielId
        };
        _profileDb.RoomUser.Add(roomUser);
        await _profileDb.SaveChangesAsync();

        return roomUser;
    }
}