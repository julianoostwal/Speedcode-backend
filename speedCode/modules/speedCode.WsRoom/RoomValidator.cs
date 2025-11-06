using Microsoft.EntityFrameworkCore;
using speedCode.Profile;
using speedCode.Room;
using speedCode.rooms;

namespace speedCode.Exercise;

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
        var room = await _db.rooms
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.id == roomId);

        if (room == null)
            return false;

        if (!room.isPrivate)
            return true;

        return await _db.RoomUser.AnyAsync(ru =>
            ru.roomId == roomId && ru.profileId == clientId
        );
    }


    public async Task<bool> isRoomAdmin(Guid roomId, Guid clientId)
    {
        WSRoomsService.Rooms.TryGetValue(roomId, out var room);
        if (room != null)
        {
            return true;
        }

        var dbRoom = await _db.rooms
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.id == roomId);

        return dbRoom.CreatedById == clientId;
    }

    public bool IsRoomStarted(Guid roomId)
    {
        // Room should always exist due to GetOrCreateRoom logic
        // If it doesn't exist in WS service yet, it means it's not started
        if (!WSRoomsService.Rooms.TryGetValue(roomId, out var room))
        {
            return false;
        }

        return room.IsStarted;
    }
}