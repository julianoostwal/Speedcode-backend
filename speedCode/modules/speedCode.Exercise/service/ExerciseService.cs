using Microsoft.EntityFrameworkCore;
using speedCode.Core.judge0.dto;
using speedCode.Core.judge0.enums;
using speedCode.exercise;
using speedCode.Exercise.model;
using speedCode.Judge0;

namespace speedCode.Exercise.service;

public class ExerciseService
{
    private readonly Judge0Handler _judge0Handler;
    private readonly ExerciseDb _dbContext;

    public ExerciseService(Judge0Handler judge0Handler, ExerciseDb dbContext)
    {
        _judge0Handler = judge0Handler;
        _dbContext = dbContext;
    }

    public List<model.Exercise> gerRandomExercises(int count)
    {
        return _dbContext.Exercise
            .OrderBy(e => EF.Functions.Random())
            .Take(count)
            .ToList();
    }

    public async Task<TestCodeAsyncResults> TestCodeAsync(TestCodeRequest request)
    {
        var results = new TestCodeAsyncResults();

        var testCodes = await _dbContext.Exercise
            .Include(e => e.ExerciseTests)
            .ThenInclude(et => et.test)
            .ThenInclude(t => t.TestExpectIOs)
            .Where(e => e.id == request.ExerciseId)
            .SelectMany(e => e.ExerciseTests.Select(et => et.test))
            .ToListAsync();

        var outputs = await RunAllTestsAsync(request, testCodes);

        results.TestProcess.AddRange(outputs);
        return results;
    }

    private async Task<List<JudgeCodeTestResults>> RunAllTestsAsync(
        TestCodeRequest request,
        List<TestCode> testCodes)
    {
        var testTasks = new List<Task<JudgeCodeTestResults>>();

        foreach (var testCode in testCodes.Where(t => t.LanguageId == request.LanguageId))
        {
            foreach (var testExpectIO in testCode.TestExpectIOs)
            {
                testTasks.Add(RunSingleTestAsync(request, testCode, testExpectIO));
            }
        }

        var outputs = await Task.WhenAll(testTasks);
        return outputs.ToList();
    }


    private async Task<JudgeCodeTestResults> RunSingleTestAsync(
        TestCodeRequest request,
        TestCode testCode,
        TestExpectIO testExpectIO)
    {
        try
        {
            var judgeCode = JudgeCodeFactory.CreateJudgeCode(testCode.Type);
            judgeCode.ExpectedResult = testExpectIO.output;

            var submissionResult = await _judge0Handler.HandleSubmitCodeAsync(
                new SubmissionRequest
                {
                    SourceCode = request.Code,
                    LanguageId = request.LanguageId,
                    Stdin = testExpectIO.input
                },
                testCode.ACRegex
            );

            judgeCode.SubmissionResult = submissionResult;

            return judgeCode.TestOutput();
        }
        catch (Exception ex)
        {
            return new JudgeCodeTestResults
            {
                Status = JudgeCodeTestStatus.Error
            };
        }
    }
}