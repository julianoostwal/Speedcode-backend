using speedCode.Room.Model;
using speedCode.Room.dtos;

namespace speedCode.Room.Interfaces;

public interface IRoomRepository
{
    Task<Rooms?> GetByIdAsync(Guid id);
    Task<List<Rooms>> GetAllAsync();
    Task<Rooms> CreateAsync(Rooms room);
    Task<Rooms> UpdateAsync(Rooms room);
    Task<bool> DeleteAsync(Guid id);
    
    Task<List<RoomUserDto>> GetRoomUsersByRoomIdAsync(Guid roomId);
    Task<List<RoomUserDto>> GetRoomUsersByProfileIdAsync(Guid profileId);
    Task<RoomUserDto> CreateRoomUserAsync(CreateRoomUserDto roomUser);
    Task<bool> DeleteRoomUserAsync(Guid id);
    Task DeleteRoomUsersByRoomIdAsync(Guid roomId);
    
    Task<List<RoomExerciseDto>> GetRoomExercisesByRoomIdAsync(Guid roomId);
    Task<RoomExerciseDto> CreateRoomExerciseAsync(CreateRoomExerciseDto roomExercise);
    Task<bool> DeleteRoomExerciseAsync(Guid id);
    Task DeleteRoomExercisesByRoomIdAsync(Guid roomId);
    Task<List<Guid>> GetExerciseIdsByRoomIdAsync(Guid roomId);
}
