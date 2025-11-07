using speedCode.Core.interfaces;
using speedCode.Room.Interfaces;

namespace speedCode.Room.service;

public class RoomDataService : IRoomDataService
{
    private readonly IRoomRepository _roomRepository;

    public RoomDataService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<List<Guid>> GetFinishedExercisesByRoomAndUserAsync(Guid roomId, Guid userId)
    {
        return await _roomRepository.GetExerciseIdsByRoomIdAsync(roomId);
    }
}
