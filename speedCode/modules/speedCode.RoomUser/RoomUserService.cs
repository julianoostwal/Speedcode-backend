using speedCode.Data;

namespace speedCode.services;

public class RoomUserService
{
    private readonly AppDbContext _db;

    public RoomUserService(AppDbContext db)
    {
        _db = db;
    }

    public List<Guid> GetAllFinishedExcisesByRoomAndUserId(Guid roomId, Guid profileId)
    {
        return (from pe in _db.ProfileExercise
                join re in _db.RoomExercise
                    on pe.exerciseId equals re.exerciseId
                where pe.profileId == profileId && re.roomId == roomId
                select pe.exerciseId)
            .ToList();
    }
}