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

    public async Task<bool> RoomExistsAsync(Guid roomId)
    {
        return await _db.Rooms.AnyAsync(r => r.id == roomId);
    }

    public async Task<bool> ClientExistsAsync(Guid clientId)
    {
        return await _db.Profiles.AnyAsync(p => p.id == clientId);
    }
}