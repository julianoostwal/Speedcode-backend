using Microsoft.EntityFrameworkCore;
using speedCode.Core.interfaces;

namespace speedCode.Room.service;

public class RoomValidationService : IRoomValidationService
{
    private readonly RoomsDb _db;

    public RoomValidationService(RoomsDb db)
    {
        _db = db;
    }

    public async Task<bool> RoomExistsAsync(Guid roomId)
    {
        return await _db.rooms.AnyAsync(r => r.id == roomId);
    }

    public async Task<bool> ClientExistsAsync(Guid clientId)
    {
        return await _db.RoomUser.AnyAsync(ru => ru.profileId == clientId);
    }

    public async Task<bool> ClientAllowToJoinAsync(Guid roomId, Guid clientId)
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

    public async Task<bool> IsRoomAdminAsync(Guid roomId, Guid clientId)
    {
        var dbRoom = await _db.rooms.FirstOrDefaultAsync(r => r.id == roomId);
        return dbRoom?.CreatedById == clientId;
    }
}
