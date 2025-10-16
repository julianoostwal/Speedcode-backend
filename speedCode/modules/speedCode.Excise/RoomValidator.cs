using Microsoft.EntityFrameworkCore;
using speedCode.Excise;
using speedCode.Profile;

namespace speedCode.Excise;

public class RoomValidator
{
    private readonly ExciseDb _db;
    private readonly ProfileDb _profileDb;

    public RoomValidator(ExciseDb db)
    {
        _db = db;
    }

    public async Task<bool> RoomExistsAsync(Guid roomId)
    {
        return await _db.Rooms.AnyAsync(r => r.id == roomId);
    }

    public async Task<bool> ClientExistsAsync(Guid clientId)
    {
        return await _profileDb.Profiles.AnyAsync(p => p.id == clientId);
    }
}