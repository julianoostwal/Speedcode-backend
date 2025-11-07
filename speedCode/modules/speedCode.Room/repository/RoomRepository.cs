using Microsoft.EntityFrameworkCore;
using speedCode.Room.dtos;
using speedCode.Room.Interfaces;
using speedCode.Room.Model;

namespace speedCode.Room.repository;

public class RoomRepository : IRoomRepository
{
    private readonly RoomsDb _dbContext;

    public RoomRepository(RoomsDb dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Rooms?> GetByIdAsync(Guid id)
    {
        return await _dbContext.rooms.FindAsync(id);
    }

    public async Task<List<Rooms>> GetAllAsync()
    {
        return await _dbContext.rooms.ToListAsync();
    }

    public async Task<Rooms> CreateAsync(Rooms room)
    {
        _dbContext.rooms.Add(room);
        await _dbContext.SaveChangesAsync();
        return room;
    }

    public async Task<Rooms> UpdateAsync(Rooms room)
    {
        _dbContext.rooms.Update(room);
        await _dbContext.SaveChangesAsync();
        return room;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var room = await _dbContext.rooms.FindAsync(id);
        if (room == null) return false;
        
        _dbContext.rooms.Remove(room);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<RoomUserDto>> GetRoomUsersByRoomIdAsync(Guid roomId)
    {
        return await _dbContext.RoomUser
            .Where(ru => ru.roomId == roomId)
            .Select(ru => MapToRoomUserDto(ru))
            .ToListAsync();
    }

    public async Task<List<RoomUserDto>> GetRoomUsersByProfileIdAsync(Guid profileId)
    {
        return await _dbContext.RoomUser
            .Where(ru => ru.profileId == profileId)
            .Select(ru => MapToRoomUserDto(ru))
            .ToListAsync();
    }

    public async Task<RoomUserDto> CreateRoomUserAsync(CreateRoomUserDto roomUserDto)
    {
        var roomUser = new RoomUser
        {
            roomId = roomUserDto.RoomId,
            profileId = roomUserDto.ProfileId,
            createdAt = DateTime.UtcNow
        };
        
        _dbContext.RoomUser.Add(roomUser);
        await _dbContext.SaveChangesAsync();
        return MapToRoomUserDto(roomUser);
    }

    public async Task<bool> DeleteRoomUserAsync(Guid id)
    {
        var roomUser = await _dbContext.RoomUser.FindAsync(id);
        if (roomUser == null) return false;
        
        _dbContext.RoomUser.Remove(roomUser);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task DeleteRoomUsersByRoomIdAsync(Guid roomId)
    {
        var roomUsers = await _dbContext.RoomUser
            .Where(ru => ru.roomId == roomId)
            .ToListAsync();
        
        _dbContext.RoomUser.RemoveRange(roomUsers);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<RoomExerciseDto>> GetRoomExercisesByRoomIdAsync(Guid roomId)
    {
        return await _dbContext.RoomExercise
            .Where(re => re.roomId == roomId)
            .Select(re => MapToRoomExerciseDto(re))
            .ToListAsync();
    }

    public async Task<RoomExerciseDto> CreateRoomExerciseAsync(CreateRoomExerciseDto roomExerciseDto)
    {
        var roomExercise = new RoomExercise
        {
            roomId = roomExerciseDto.RoomId,
            exerciseId = roomExerciseDto.ExerciseId
        };
        
        _dbContext.RoomExercise.Add(roomExercise);
        await _dbContext.SaveChangesAsync();
        return MapToRoomExerciseDto(roomExercise);
    }

    public async Task<bool> DeleteRoomExerciseAsync(Guid id)
    {
        var roomExercise = await _dbContext.RoomExercise.FindAsync(id);
        if (roomExercise == null) return false;
        
        _dbContext.RoomExercise.Remove(roomExercise);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task DeleteRoomExercisesByRoomIdAsync(Guid roomId)
    {
        var roomExercises = await _dbContext.RoomExercise
            .Where(re => re.roomId == roomId)
            .ToListAsync();
        
        _dbContext.RoomExercise.RemoveRange(roomExercises);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Guid>> GetExerciseIdsByRoomIdAsync(Guid roomId)
    {
        return await _dbContext.RoomExercise
            .Where(re => re.roomId == roomId)
            .Select(re => re.exerciseId)
            .ToListAsync();
    }

    private static RoomUserDto MapToRoomUserDto(RoomUser roomUser)
    {
        return new RoomUserDto
        {
            Id = roomUser.id,
            RoomId = roomUser.roomId,
            ProfileId = roomUser.profileId,
            CreatedAt = roomUser.createdAt
        };
    }

    private static RoomExerciseDto MapToRoomExerciseDto(RoomExercise roomExercise)
    {
        return new RoomExerciseDto
        {
            Id = roomExercise.id,
            RoomId = roomExercise.roomId,
            ExerciseId = roomExercise.exerciseId
        };
    }
}
