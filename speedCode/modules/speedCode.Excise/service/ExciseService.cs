using Microsoft.EntityFrameworkCore;
using speedCode.codeTesting;
using speedCode.Core.judge0.dto;
using speedCode.Excise;
using speedCode.Excise.model;
using speedCode.Judge0;
using speedCode.Model;
using speedCode.rooms;
using speedCode.Room;
using speedCode.Profile;

namespace speedCode.services;

public class ExciseService
{
    private readonly Judge0Client _judge0Client;
    private readonly ExciseDb _dbContext;
    private readonly RoomsDb _roomsDb;
    private readonly ProfileDb _roomProfilesDb;

    public ExciseService(Judge0Client judge0Client, ExciseDb dbContext, RoomsDb roomsDb, ProfileDb roomProfilesDb)
    {
        _judge0Client = judge0Client;
        _dbContext = dbContext;
        _roomsDb = roomsDb;
        _roomProfilesDb = roomProfilesDb;
    }

    public List<Guid> GetAllFinishedExcisesByRoomAndUserId(Guid roomId, Guid profileId)
    {
        return (from pe in _dbContext.ProfileExercise
                join re in _dbContext.RoomExercise
                    on pe.exerciseId equals re.exerciseId
                where pe.profileId == profileId && re.roomId == roomId
                select pe.exerciseId)
            .ToList();
    }

    public async Task<TestCodeAsyncResults> TestCodeAsyncForWSRoom(TestCodeRequest request, Guid roomId,
        Guid clientId)
    {
        TestCodeAsyncResults result = await TestCodeAsync(request);

        if (result.IsPassed)
        {
            if (WSRoomsService.Rooms.TryGetValue(roomId, out var roomValue))
            {
                PlayerManager.CompletedAExercise(roomValue, clientId, request.ExerciseId);
            }

            ProfileExercise profileExercise = new ProfileExercise()
            {
                profileId = clientId,
                exerciseId = request.ExerciseId
            };
            _dbContext.ProfileExercise.Add(profileExercise);
            await _dbContext.SaveChangesAsync();
        }

        return result;
    }

    public async Task<TestCodeAsyncResults> TestCodeAsync(TestCodeRequest request)
    {
        TestCodeAsyncResults results = new TestCodeAsyncResults();
        List<TestCode> testCodes = await _dbContext.Exercise
            .Include(e => e.ExerciseTests)
            .ThenInclude(et => et.test)
            .ThenInclude(t => t.TestExpectIOs)
            .Where(e => e.id == request.ExerciseId)
            .SelectMany(e => e.ExerciseTests.Select(et => et.test))
            .ToListAsync();


        foreach (var testCode in testCodes)
        {
            foreach (var testExpectIO in testCode.TestExpectIOs)
            {
                JudgeCode judgeCodeFactoryResults = JudgeCodeFactory.CreateJudgeCode(testCode.Type);
                judgeCodeFactoryResults.ExpectedResult = testExpectIO.output;
                judgeCodeFactoryResults.Input = testExpectIO.input;
                judgeCodeFactoryResults.OutPut = _judge0Client
                    .SubmitCodeAsync(new SubmissionRequest(request.Code, request.LanguageId, testExpectIO.input)).Result
                    .Stdout;
                JudgeCodeTestResults output = judgeCodeFactoryResults.TestOutput();
                results.TestProcess.Add(output);
            }
        }

        return await Task.FromResult(results);
    }

    public async Task<List<object>> GetAllRooms()
    {
        var rooms = await _roomsDb.rooms.ToListAsync();
        var exercises = await _dbContext.Exercise.ToListAsync();
        var roomExcises = await _dbContext.RoomExercise.ToListAsync();
        var roomProfiles = await _roomProfilesDb.RoomUser.ToListAsync();
        var profiles = await _roomProfilesDb.Profiles.ToListAsync();

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
}