using Microsoft.EntityFrameworkCore;
using speedCode.Core.judge0.dto;
using speedCode.Excise;
using speedCode.Excise.dtos;
using speedCode.Excise.model;
using speedCode.Judge0;

namespace speedCode.services;

public class ExciseService
{
    private readonly Judge0Handler _judge0Handler;
    private readonly ExciseDb _dbContext;

    public ExciseService(Judge0Handler judge0Handler, ExciseDb dbContext)
    {
        _judge0Handler = judge0Handler;
        _dbContext = dbContext;
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
                    .HandleSubmitCodeAsync(new SubmissionRequest()
                    {
                        SourceCode = request.Code,
                        LanguageId = request.LanguageId,
                        Stdin = testExpectIO.input
                    },
                        testCode.ACRegex).Result;
                JudgeCodeTestResults output = judgeCodeFactoryResults.TestOutput();
                results.TestProcess.Add(output);
            }
        }
        return await Task.FromResult(results);
    }
    
    //! moet nog verder uitgewerkt worden
    /*
    public async Task<List<ExerciseFullDto>> GetAllExercises()
    {

        var exercises = _dbContext.Exercise
            .Include(e => e.ExerciseTests)
            .ToList();

        var exerciseDtos = exercises.Select(e => new ExerciseFullDto
        {
            id = e.id,
            title = e.title,
            description = e.description,
            isRoomExcise = e.isRoomExcise,
            isPrivate = e.isPrivate,
            points = e.points,
            level = e.Level.ToString(),
            createdAt = e.createdAt
        }).ToList();

        return exerciseDtos;
    }
    */
}