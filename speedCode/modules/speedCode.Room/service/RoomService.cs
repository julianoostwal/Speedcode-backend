using Microsoft.EntityFrameworkCore;
using speedCode.Core.rooms.dto;
using speedCode.Core.rooms.Mapper;
using speedCode.Room.dtos;
using speedCode.Room.Interfaces;
using speedCode.Room.Model;
using speedCode.Core.interfaces;

namespace speedCode.Room.service;

public class RoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IProfileQueryService _profileQueryService;
    private readonly IExerciseQueryService _exerciseQueryService;
    private readonly IWSRoomStatusProvider _wsRoomStatusProvider;

    public RoomService(
        IRoomRepository roomRepository,
        IProfileQueryService profileQueryService,
        IExerciseQueryService exerciseQueryService,
        IWSRoomStatusProvider wsRoomStatusProvider)
    {
        _roomRepository = roomRepository;
        _profileQueryService = profileQueryService;
        _exerciseQueryService = exerciseQueryService;
        _wsRoomStatusProvider = wsRoomStatusProvider;
    }

    public async Task<Rooms> DeleteRoom(Guid id)
    {
        var room = await _roomRepository.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException($"Room {id} not found");
        await _roomRepository.DeleteAsync(id);
        return room;
    }

    public async Task<Rooms> UpdateRoom(Guid id, UpdateRoomDTO dto)
    {
        var room = await _roomRepository.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException($"Room {id} not found");

        room.roomName = dto.RoomName;
        room.isPrivate = dto.IsPrivate;
        room.roomLevel = dto.RoomLevel;

        await _roomRepository.DeleteRoomExercisesByRoomIdAsync(id);
        await _roomRepository.DeleteRoomUsersByRoomIdAsync(id);

        foreach (var exerciseId in dto.exerciseId)
        {
            await _roomRepository.CreateRoomExerciseAsync(
                new CreateRoomExerciseDto { RoomId = room.id, ExerciseId = exerciseId });
        }

        foreach (var profileId in dto.profileId)
        {
            await _roomRepository.CreateRoomUserAsync(
                new CreateRoomUserDto { RoomId = room.id, ProfileId = profileId });
        }

        await _roomRepository.UpdateAsync(room);
        return room;
    }

    public async Task<Rooms> CreateRoom(CreateRoomDTO roomRequest)
    {
        Rooms room = RoomMapper.FromCreateRoomDto(roomRequest);
        await _roomRepository.CreateAsync(room);
        await _roomRepository.CreateRoomUserAsync(new CreateRoomUserDto
        {
            RoomId = room.id,
            ProfileId = roomRequest.profileId
        });
        return room;
    }

    public async Task<List<Guid>> GetAllFinishedexercisesByRoomAndUserId(Guid roomId, Guid profileId)
    {
        var roomExerciseIds = await _roomRepository.GetExerciseIdsByRoomIdAsync(roomId);
        return roomExerciseIds;
    }

    public async Task<Rooms> GenerateRoomWithRandomexercises(CreateRoomDTO roomRequest, int count)
    {
        Rooms room = RoomMapper.FromCreateRoomDto(roomRequest);
        await _roomRepository.CreateAsync(room);

        var exercises = await _exerciseQueryService.GetAllAsync();
        var randomExercises = exercises.Take(count).ToList();

        foreach (var ex in randomExercises)
        {
            await _roomRepository.CreateRoomExerciseAsync(new CreateRoomExerciseDto
            {
                RoomId = room.id,
                ExerciseId = ex.Id
            });
        }

        await _roomRepository.CreateRoomUserAsync(new CreateRoomUserDto
        {
            RoomId = room.id,
            ProfileId = roomRequest.profileId
        });

        return room;
    }

    public async Task<List<RoomFullDto>> GetAllRooms(Guid userId)
    {
        var rooms = await _roomRepository.GetAllAsync();
        var allRoomUsers = new List<RoomUserDto>();
        var allRoomExercises = new List<RoomExerciseDto>();

        // Load all relationships
        foreach (var room in rooms)
        {
            var roomUsers = await _roomRepository.GetRoomUsersByRoomIdAsync(room.id);
            var roomExercises = await _roomRepository.GetRoomExercisesByRoomIdAsync(room.id);
            allRoomUsers.AddRange(roomUsers);
            allRoomExercises.AddRange(roomExercises);
        }

        var filteredRooms = rooms.Where(room =>
        {
            var roomUsers = allRoomUsers.Where(ru => ru.RoomId == room.id).ToList();
            return !room.isPrivate || roomUsers.Any(u => u.ProfileId == userId);
        }).ToList();

        var allProfileIds = filteredRooms
            .Select(r => r.CreatedById)
            .Concat(allRoomUsers.Where(ru => filteredRooms.Any(r => r.id == ru.RoomId))
                .Select(ru => ru.ProfileId))
            .Distinct()
            .ToList();

        var profiles = await _profileQueryService.GetByIdsAsync(allProfileIds);

        var exerciseIds = allRoomExercises
            .Where(re => filteredRooms.Any(r => r.id == re.RoomId))
            .Select(re => re.ExerciseId)
            .Distinct()
            .ToList();

        var exercises = await _exerciseQueryService.GetByIdsAsync(exerciseIds);

        var roomDtos = filteredRooms.Select(room =>
        {
            var roomUsers = allRoomUsers.Where(ru => ru.RoomId == room.id).ToList();
            var roomExercises = allRoomExercises.Where(re => re.RoomId == room.id).ToList();

            var (isStarted, activePlayers) = _wsRoomStatusProvider.GetRoomStatus(room.id);

            return new RoomFullDto
            {
                id = room.id,
                roomName = room.roomName,
                roomLevel = room.roomLevel.ToString(),
                isPrivate = room.isPrivate,
                createdAt = room.createdAt,
                CreatedById = room.CreatedById,
                IsStarted = isStarted,
                ActivePlayers = activePlayers,
                CreatedBy = profiles
                    .Where(p => p.Id == room.CreatedById)
                    .Select(p => new RoomFullDto.ProfileDto
                    {
                        id = p.Id,
                        avatarUrl = p.AvatarUrl,
                        createdAt = p.CreatedAt,
                        nickName = p.NickName,
                        role = p.Role
                    })
                    .FirstOrDefault(),
                RoomUsers = roomUsers.Select(ru => new RoomFullDto.RoomUserDto
                {
                    id = ru.Id,
                    roomId = ru.RoomId,
                    profileId = ru.ProfileId,
                    createdAt = ru.CreatedAt,
                    profile = profiles
                        .Where(p => p.Id == ru.ProfileId)
                        .Select(p => new RoomFullDto.ProfileDto
                        {
                            id = p.Id,
                            avatarUrl = p.AvatarUrl,
                            createdAt = p.CreatedAt,
                            nickName = p.NickName,
                            role = p.Role
                        })
                        .FirstOrDefault()
                }).ToList(),
                RoomExercises = roomExercises.Select(re => new RoomFullDto.RoomExerciseDto
                {
                    id = re.Id,
                    roomId = re.RoomId,
                    exerciseId = re.ExerciseId,
                    exercise = exercises
                        .Where(e => e.Id == re.ExerciseId)
                        .Select(e => new RoomFullDto.ExerciseDto
                        {
                            id = e.Id,
                            title = e.Title,
                            description = e.Description,
                            createdAt = e.CreatedAt,
                            isRoomExercise = false,
                            isPrivate = e.IsPrivate,
                            points = 0,
                            Level = e.Difficulty
                        })
                        .FirstOrDefault()
                }).ToList()
            };
        }).ToList();

        return roomDtos;
    }

    public async Task<Rooms?> GetRoomById(Guid id)
    {
        return await _roomRepository.GetByIdAsync(id);
    }

    public async Task<RoomFullDto> GetRoomAndKoppelById(Guid id)
    {
        var room = await _roomRepository.GetByIdAsync(id);

        if (room == null) return null;

        var roomUsers = await _roomRepository.GetRoomUsersByRoomIdAsync(id);
        var roomExercises = await _roomRepository.GetRoomExercisesByRoomIdAsync(id);

        var allProfileIds = new List<Guid> { room.CreatedById };
        allProfileIds.AddRange(roomUsers.Select(ru => ru.ProfileId));
        allProfileIds = allProfileIds.Distinct().ToList();

        var profiles = await _profileQueryService.GetByIdsAsync(allProfileIds);

        var exerciseIds = roomExercises.Select(re => re.ExerciseId).Distinct().ToList();
        var exercises = await _exerciseQueryService.GetByIdsAsync(exerciseIds);

        var roomDto = new RoomFullDto
        {
            id = room.id,
            roomName = room.roomName,
            roomLevel = room.roomLevel.ToString(),
            isPrivate = room.isPrivate,
            createdAt = room.createdAt,
            CreatedById = room.CreatedById,
            RoomUsers = roomUsers.Select(ru => new RoomFullDto.RoomUserDto
            {
                id = ru.Id,
                roomId = ru.RoomId,
                profileId = ru.ProfileId,
                createdAt = ru.CreatedAt,
                profile = profiles
                    .Where(p => p.Id == ru.ProfileId)
                    .Select(p => new RoomFullDto.ProfileDto
                    {
                        id = p.Id,
                        avatarUrl = p.AvatarUrl,
                        createdAt = p.CreatedAt,
                        nickName = p.NickName,
                        role = p.Role
                    })
                    .FirstOrDefault()
            }).ToList(),
            RoomExercises = roomExercises.Select(re => new RoomFullDto.RoomExerciseDto
            {
                id = re.Id,
                roomId = re.RoomId,
                exerciseId = re.ExerciseId,
                exercise = exercises
                    .Where(e => e.Id == re.ExerciseId)
                    .Select(e => new RoomFullDto.ExerciseDto
                    {
                        id = e.Id,
                        title = e.Title,
                        description = e.Description,
                        createdAt = e.CreatedAt,
                        isRoomExercise = false,
                        isPrivate = e.IsPrivate,
                        points = 0,
                        Level = e.Difficulty
                    })
                    .FirstOrDefault()
            }).ToList()
        };

        return roomDto;
    }
}