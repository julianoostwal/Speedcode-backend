using Microsoft.EntityFrameworkCore;
using speedCode.Data;

namespace speedCode.rooms;

public class RoomValidator
{
    private readonly AppDbContext _db;

    public RoomValidator(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> RoomExistsAsync(string roomId)
    {
        if (!Guid.TryParse(roomId, out var roomGuid)) return false;
        return await _db.Rooms.AnyAsync(r => r.id == roomGuid);
    }

    public async Task<bool> ClientExistsAsync(string clientId)
    {
        if (!Guid.TryParse(clientId, out var clientGuid)) return false;
        return await _db.Profiles.AnyAsync(p => p.id == clientGuid);
    }
}