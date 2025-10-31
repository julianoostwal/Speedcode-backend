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

    public async Task<List<RoomFullDto>> GetAllRooms()
    {
        var rooms = await _RoomDb.rooms
            .Include(r => r.RoomExercises)
                .ThenInclude(re => re.exercise)
            .Include(r => r.RoomUsers)
            .ToListAsync();
    
        var allProfileIds = rooms
            .Select(r => r.CreatedById)
            .Concat(rooms.SelectMany(r => r.RoomUsers.Select(ru => ru.profileId)))
            .Distinct()
            .ToList();
    
        var profiles = await _profileDb.Profiles
            .Where(p => allProfileIds.Contains(p.id))
            .ToListAsync();
    
        var roomDtos = rooms.Select(room => new RoomFullDto
        {
            id = room.id,
            roomName = room.roomName,
            roomLevel = room.roomLevel.ToString(),
            isPrivate = room.isPrivate,
            createdAt = room.createdAt,
            CreatedById = room.CreatedById,
            CreatedBy = profiles
                .Where(p => p.id == room.CreatedById)
                .Select(p => new RoomFullDto.ProfileDto
                {
                    id = p.id,
                    avatarUrl = p.avatarUrl,
                    createdAt = p.createdAt,
                    nickName = p.nickName,
                    role = p.role.ToString()
                })
                .FirstOrDefault(),
            RoomUsers = room.RoomUsers.Select(ru => new RoomFullDto.RoomUserDto
            {
                id = ru.id,
                roomId = ru.roomId,
                profileId = ru.profileId,
                createdAt = ru.createdAt,
                profile = profiles
                    .Where(p => p.id == ru.profileId)
                    .Select(p => new RoomFullDto.ProfileDto
                    {
                        id = p.id,
                        avatarUrl = p.avatarUrl,
                        createdAt = p.createdAt,
                        nickName = p.nickName,
                        role = p.role.ToString()
                    })
                    .FirstOrDefault()
            }).ToList(),
            RoomExercises = room.RoomExercises.Select(re => new RoomFullDto.RoomExerciseDto
            {
                id = re.id,
                roomId = re.roomId,
                exerciseId = re.exerciseId,
                exercise = re.exercise == null ? null : new RoomFullDto.ExerciseDto
                {
                    id = re.exercise.id,
                    title = re.exercise.title,
                    description = re.exercise.description,
                    createdAt = re.exercise.createdAt,
                    isRoomExcise = re.exercise.isRoomExcise,
                    isPrivate = re.exercise.isPrivate,
                    points = re.exercise.points,
                    Level = re.exercise.Level.ToString()
                }
            }).ToList()
        }).ToList();
    
        return roomDtos;
    }

    public async Task<RoomFullDto?> GetRoomById(Guid id)
    {
        var room = await _RoomDb.rooms
            .Include(r => r.RoomExercises).ThenInclude(re => re.exercise)
            .Include(r => r.RoomUsers)
            .FirstOrDefaultAsync(r => r.id == id);
    
        if (room == null) return null;
    
        var allProfileIds = new List<Guid> { room.CreatedById };
        allProfileIds.AddRange(room.RoomUsers.Select(ru => ru.profileId));
        allProfileIds = allProfileIds.Distinct().ToList();
    
        var profiles = await _profileDb.Profiles
            .Where(p => allProfileIds.Contains(p.id))
            .ToListAsync();
    
        return new RoomFullDto
        {
            id = room.id,
            roomName = room.roomName,
            roomLevel = room.roomLevel.ToString(),
            isPrivate = room.isPrivate,
            createdAt = room.createdAt,
            CreatedById = room.CreatedById,
            CreatedBy = profiles
                .Where(p => p.id == room.CreatedById)
                .Select(p => new RoomFullDto.ProfileDto
                {
                    id = p.id,
                    avatarUrl = p.avatarUrl,
                    createdAt = p.createdAt,
                    nickName = p.nickName,
                    role = p.role.ToString()
                })
                .FirstOrDefault(),
            RoomUsers = room.RoomUsers.Select(ru => new RoomFullDto.RoomUserDto
            {
                id = ru.id,
                roomId = ru.roomId,
                profileId = ru.profileId,
                createdAt = ru.createdAt,
                profile = profiles
                    .Where(p => p.id == ru.profileId)
                    .Select(p => new RoomFullDto.ProfileDto
                    {
                        id = p.id,
                        avatarUrl = p.avatarUrl,
                        createdAt = p.createdAt,
                        nickName = p.nickName,
                        role = p.role.ToString()
                    })
                    .FirstOrDefault()
            }).ToList(),
            RoomExercises = room.RoomExercises.Select(re => new RoomFullDto.RoomExerciseDto
            {
                id = re.id,
                roomId = re.roomId,
                exerciseId = re.exerciseId,
                exercise = re.exercise == null ? null : new RoomFullDto.ExerciseDto
                {
                    id = re.exercise.id,
                    title = re.exercise.title,
                    description = re.exercise.description,
                    createdAt = re.exercise.createdAt,
                    isRoomExcise = re.exercise.isRoomExcise,
                    isPrivate = re.exercise.isPrivate,
                    points = re.exercise.points,
                    Level = re.exercise.Level.ToString()
                }
            }).ToList()
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