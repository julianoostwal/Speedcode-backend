using Microsoft.EntityFrameworkCore;
using speedCode.codeTesting;
using speedCode.Core.judge0.dto;
using speedCode.Excise;
using speedCode.Judge0;
using speedCode.Model;
using speedCode.rooms;

namespace speedCode.services;

public class ExciseService
{
    private readonly Judge0Client _judge0Client;
    private readonly ExciseDb _dbContext;

    public ExciseService(Judge0Client judge0Client, ExciseDb dbContext)
    {
        _judge0Client = judge0Client;
        _dbContext = dbContext;
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
}