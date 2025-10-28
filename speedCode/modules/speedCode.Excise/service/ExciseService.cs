using Microsoft.EntityFrameworkCore;
using speedCode.codeTesting;
using speedCode.Core.judge0.dto;
using speedCode.Excise;
using speedCode.Excise.model;
using speedCode.Judge0;
<<<<<<< HEAD
using speedCode.Model;
using speedCode.rooms;
using speedCode.Room;
using speedCode.Profile;
=======
>>>>>>> main

namespace speedCode.services;

public class ExciseService
{
    private readonly Judge0Handler _judge0Handler;
    private readonly ExciseDb _dbContext;
    private readonly RoomsDb _roomsDb;
    private readonly ProfileDb _roomProfilesDb;

<<<<<<< HEAD
    public ExciseService(Judge0Client judge0Client, ExciseDb dbContext, RoomsDb roomsDb, ProfileDb roomProfilesDb)
=======
    public ExciseService(Judge0Handler judge0Handler, ExciseDb dbContext)
>>>>>>> main
    {
        _judge0Handler = judge0Handler;
        _dbContext = dbContext;
        _roomsDb = roomsDb;
        _roomProfilesDb = roomProfilesDb;
    }

    public List<Exercise> gerRandomExercises(int count)
    {
        return _dbContext.Exercise
            .OrderBy(e => EF.Functions.Random())
            .Take(count)
            .ToList();
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
                judgeCodeFactoryResults.SubmissionResult = _judge0Handler
                    .HandleSubmitCodeAsync(new SubmissionRequest(request.Code, request.LanguageId, testExpectIO.input),
                        testCode.ACRegex).Result;
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