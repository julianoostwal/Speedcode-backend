namespace speedCode.Core.interfaces;

public interface IRoomDataService
{
    Task<List<Guid>> GetFinishedExercisesByRoomAndUserAsync(Guid roomId, Guid userId);
}


