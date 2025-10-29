using Microsoft.EntityFrameworkCore;
using speedCode.Core.rooms.dto;
using speedCode.Core.rooms.Mapper;
using speedCode.Excise.model;
using speedCode.Profile;
using speedCode.Room;
using speedCode.Room.Model;
using speedCode.services;

namespace speedCode.Excise.service;

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

    public async Task<Rooms> CreateRoom(CreateRoomDTO roomRequest)
    {
        Rooms room = RoomMapper.FromCreateRoomDto(roomRequest);
        _RoomDb.rooms.Add(room);
        await _RoomDb.SaveChangesAsync();
        return room;
    }

    public List<Guid> GetAllFinishedExcisesByRoomAndUserId(Guid roomId, Guid profileId)
    {
        return (from pe in _profileDb.ProfileExercise
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
}