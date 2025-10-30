using Microsoft.EntityFrameworkCore;
using speedCode.Excise;
using speedCode.Profile;
using speedCode.Room;
using speedCode.rooms;

namespace speedCode.Excise;

public class RoomValidator
{
    private readonly RoomsDb _db;
    private readonly ProfileDb _profileDb;

    public RoomValidator(RoomsDb db, ProfileDb profileDb)
    {
        _db = db;
        _profileDb = profileDb;
    }

    public async Task<bool> RoomExistsAsync(Guid roomId)
    {
        return await _db.rooms.AnyAsync(r => r.id == roomId);
    }

    public async Task<bool> ClientExistsAsync(Guid clientId)
    {
        return await _profileDb.Profiles.AnyAsync(p => p.id == clientId);
    }

    public async Task<bool> ClientAllowToJoin(Guid roomId, Guid clientId)
    {
        return await _db.RoomUser.AnyAsync(ru =>
            ru.roomId == roomId && (!ru.room.isPrivate || ru.profileId == clientId)
        );
    }

    // public async Task<bool> onlyAdminAllowToStartARoom(Guid roomId, Guid clientId)
    // {
    //     WSRoomsService.Rooms.TryGetValue(roomId, out var room);
    //     if (room)
    //     {
    //         
    //     }
    // }
}