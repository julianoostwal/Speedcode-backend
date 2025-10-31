using Microsoft.EntityFrameworkCore;
using speedCode.Core.rooms.dto;
using speedCode.Core.rooms.Mapper;
using speedCode.Excise;
using speedCode.Excise.model;
using speedCode.Profile;
using speedCode.Profile.model;
using speedCode.Room.dtos;
using speedCode.Room.Model;
using speedCode.services;

namespace speedCode.Room.service;

public class RoomService
{
    private readonly RoomsDb _RoomDb;
    private readonly ProfileDb _profileDb;
    private readonly ExciseService _exciseService;
    private readonly ExciseDb _exciseDb;

    public RoomService(RoomsDb db, ProfileDb profileDb, ExciseService exerciseService, ExciseDb exciseDb)
    {
        _exciseService = exerciseService;
        _RoomDb = db;
        _exciseDb = exciseDb;
        _profileDb = profileDb;
    }

    public async Task<Rooms> UpdateRoom(Guid id, UpdateRoomDTO dto)
    {
        var room = _RoomDb.rooms
                       .Include(r => r.RoomExercises)
                       .Include(r => r.RoomUsers)
                       .FirstOrDefault(r => r.id == id)
                   ?? throw new KeyNotFoundException($"Room {id} not found");

        room.roomName = dto.RoomName;
        room.isPrivate = dto.IsPrivate;
        room.roomLevel = dto.RoomLevel;

        room.RoomExercises.Clear();
        room.RoomExercises = dto.exerciseId
            .Select(id => new RoomExercise { roomId = room.id, exerciseId = id })
            .ToList();

        room.RoomUsers.Clear();
        room.RoomUsers = dto.profileId
            .Select(id => new RoomUser { roomId = room.id, profileId = id })
            .ToList();

        _RoomDb.SaveChanges();

        return room;
    }

    public async Task<Rooms> CreateRoom(CreateRoomDTO roomRequest)
    {
        Rooms room = RoomMapper.FromCreateRoomDto(roomRequest);
        _RoomDb.rooms.Add(room);
        _RoomDb.RoomUser.Add(new RoomUser
        {
            roomId = room.id,
            profileId = roomRequest.profileId
        });
        await _RoomDb.SaveChangesAsync();
        return room;
    }

    public List<Guid> GetAllFinishedExcisesByRoomAndUserId(Guid roomId, Guid profileId)
    {
        var roomExerciseIds = _RoomDb.RoomExercise
            .Where(re => re.roomId == roomId)
            .Select(re => re.exerciseId)
            .ToList();

        return _profileDb.CompletedProfileExercise
            .Where(pe => pe.profileId == profileId && roomExerciseIds.Contains(pe.exerciseId))
            .Select(pe => pe.exerciseId)
            .ToList();
    }


    public Rooms GenerateRoomWithRandomExcises(CreateRoomDTO roomRequest, int count)
    {
        Rooms room = RoomMapper.FromCreateRoomDto(roomRequest);
        _RoomDb.rooms.Add(room);

        var exercises = _exciseService.gerRandomExercises(count);

        foreach (var ex in exercises)
        {
            var roomExercise = new RoomExercise
            {
                roomId = room.id,
                exerciseId = ex.id
            };
            _RoomDb.RoomExercise.Add(roomExercise);
        }

        _RoomDb.RoomUser.Add(new RoomUser
        {
            roomId = room.id,
            profileId = roomRequest.profileId
        });
        _RoomDb.SaveChanges();

        return room;
    }

    public async Task<List<object>> GetAllRooms()
    {
        var rooms = await _RoomDb.rooms.ToListAsync();
        var exercises = await _exciseDb.Exercise.ToListAsync();
        var roomExcises = await _RoomDb.RoomExercise.ToListAsync();
        var roomProfiles = await _RoomDb.RoomUser.ToListAsync();
        var profiles = await _profileDb.Profiles.ToListAsync();

        var result = new List<object>();

        foreach (var room in rooms)
        {
            var excisesForRoom = roomExcises
                .Where(re => re.roomId == room.id)
                .Select(re => exercises.FirstOrDefault(e => e.id == re.exerciseId));

            var profilesForRoom = roomProfiles
                .Where(rp => rp.roomId == room.id)
                .Select(rp => profiles.FirstOrDefault(p => p.id == rp.profileId));

            result.Add(new
            {
                room,
                profilesForRoom,
                excisesForRoom
            });
        }

        return result;
    }

    public async Task<object> GetRoomById(Guid id)
    {
        var room = await _RoomDb.rooms.FindAsync(id);
        if (room == null)
        {
            return null;
        }

        var exercises = await _exciseDb.Exercise.ToListAsync();
        var roomExcises = await _RoomDb.RoomExercise
            .Where(re => re.roomId == room.id)
            .ToListAsync();

        var excisesForRoom = roomExcises
            .Select(re => exercises.FirstOrDefault(e => e.id == re.exerciseId));

        var roomProfiles = await _RoomDb.RoomUser
            .Where(rp => rp.roomId == room.id)
            .ToListAsync();

        var profiles = await _profileDb.Profiles.ToListAsync();
        var profilesForRoom = roomProfiles
            .Select(rp => profiles.FirstOrDefault(p => p.id == rp.profileId));

        return new
        {
            room,
            profilesForRoom,
            excisesForRoom
        };
    }

    public async Task<bool> DeleteRoom(Guid id)
    {
        var room = await _RoomDb.rooms.FindAsync(id);
        var roomExercises = _RoomDb.RoomExercise.Where(re => re.roomId == id);
        var roomUsers = _RoomDb.RoomUser.Where(ru => ru.roomId == id);

        if (roomExercises.Any())
        {
            _RoomDb.RoomExercise.RemoveRange(roomExercises);
        }

        if (roomUsers.Any())
        {
            _RoomDb.RoomUser.RemoveRange(roomUsers);
        }

        if (room != null)
        {
            _RoomDb.rooms.Remove(room);
        }

        await _RoomDb.SaveChangesAsync();
        return true;
    }
}