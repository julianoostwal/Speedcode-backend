using Microsoft.EntityFrameworkCore;
using speedCode.codeTesting;
using speedCode.Core.judge0.dto;
using speedCode.Excise;
using speedCode.Excise.model;
using speedCode.Judge0;

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