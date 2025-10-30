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

    public List<RoomExercise> UpdateRoomExercises(updateRoomExercisesDTO dto)
    {
        var room = _RoomDb.rooms
            .Include(r => r.RoomExercises)
            .FirstOrDefault(r => r.id == dto.roomId);

        if (room == null) throw new Exception("Room not found");

        _RoomDb.RoomExercise.RemoveRange(room.RoomExercises);

        foreach (var exId in dto.exerciseId)
        {
            room.RoomExercises.Add(new RoomExercise
            {
                roomId = dto.roomId,
                exerciseId = exId
            });
        }

        _RoomDb.SaveChanges();
        return _RoomDb.RoomExercise
            .Where(re => re.roomId == dto.roomId)
            .ToList();
    }

    public Rooms UpdateRoom(UpdateRoomDTO dto)
    {
        UpdateRoomExercises(new updateRoomExercisesDTO()
        {
            roomId = dto.roomId,
            exerciseId = dto.exerciseId
        });
        UpdateRoomUsers(new updateRoomUsersDTO()
        {
            RoomId = dto.roomId,
            Profilesid = dto.profileId
        });
        return _RoomDb.rooms
            .Where(re => re.id == dto.roomId).FirstOrDefault();
    }

    public List<RoomUser> UpdateRoomUsers(updateRoomUsersDTO dto)
    {
        Rooms? room = _RoomDb.rooms
            .Include(r => r.RoomUsers)
            .FirstOrDefault(r => r.id == dto.RoomId);

        if (room == null) throw new Exception("Room not found");

        _RoomDb.RoomUser.RemoveRange(room.RoomUsers);

        foreach (var exId in dto.Profilesid)
        {
            room.RoomExercises.Add(new RoomExercise
            {
                roomId = dto.RoomId,
                exerciseId = exId
            });
        }

        _RoomDb.SaveChanges();
        return _RoomDb.RoomUser
            .Where(re => re.roomId == dto.RoomId)
            .ToList();
    }

    public async Task<Rooms> CreateRoom(CreateRoomDTO roomRequest)
    {
        Rooms room = RoomMapper.FromCreateRoomDto(roomRequest);
        _RoomDb.rooms.Add(room);
        await _RoomDb.SaveChangesAsync();
        return room;
    }

    public List<Guid> GetAllFinishedExcisesByRoomAndUserId(Guid roomId, Guid profileId)
    {
        return (from pe in _profileDb.CompletedProfileExercise
                join re in _RoomDb.RoomExercise
                    on pe.exerciseId equals re.exerciseId
                where pe.profileId == profileId && re.roomId == roomId
                select pe.exerciseId)
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
                id = room.id,
                roomName = room.roomName,
                roomLevel = room.roomLevel,
                isPrivate = room.isPrivate,
                chatId = room.chatId,
                createdById = room.CreatedById,
                profiles = profilesForRoom.Select(pr => new
                {
                    id = pr.id,
                    nickName = pr.nickName,
                    role = pr.role,
                    avatarUrl = pr.avatarUrl,
                    createdAt = pr.createdAt
                }),
                exercises = excisesForRoom.Select(e => new
                {
                    id = e.id,
                    title = e.title,
                    description = e.description,
                    createdAt = e.createdAt,
                    points = e.points,
                    isRoomExcise = e.isRoomExcise
                })
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
            id = room.id,
            roomName = room.roomName,
            roomLevel = room.roomLevel,
            isPrivate = room.isPrivate,
            chatId = room.chatId,
            createdById = room.CreatedById,
            profiles = profilesForRoom.Select(pr => new
            {
                id = pr.id,
                nickName = pr.nickName,
                role = pr.role,
                avatarUrl = pr.avatarUrl,
                createdAt = pr.createdAt
            }),
            exercises = excisesForRoom.Select(e => new
            {
                id = e.id,
                title = e.title,
                description = e.description,
                createdAt = e.createdAt,
                points = e.points,
                isRoomExcise = e.isRoomExcise
            })
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