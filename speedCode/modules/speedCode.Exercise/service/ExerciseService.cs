using Microsoft.EntityFrameworkCore;
using speedCode.Core.judge0.dto;
using speedCode.Core.judge0.enums;
using speedCode.exercise;
using speedCode.Exercise.Interfaces;
using speedCode.Exercise.model;
using speedCode.Judge0;

namespace speedCode.Exercise.service;

public class ExerciseService
{
    private readonly Judge0Handler _judge0Handler;
    private readonly IExerciseRepository _exerciseRepository;

    public ExerciseService(Judge0Handler judge0Handler, IExerciseRepository exerciseRepository)
    {
        _judge0Handler = judge0Handler;
        _exerciseRepository = exerciseRepository;
    }

    public List<model.Exercise> gerRandomExercises(int count)
    {
        return _exerciseRepository.GetRandomExercisesAsync(count).Result;
    }

    public async Task<TestCodeAsyncResults> TestCodeAsync(TestCodeRequest request)
    {
        var results = new TestCodeAsyncResults();

        // Use repository instead of direct DbContext access
        var testCodes = await _exerciseRepository.GetTestCodesByExerciseIdAsync(request.ExerciseId);

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
            // Use repository to get TestExpectIOs
            var testExpectIOs = await _exerciseRepository.GetTestExpectIOsByTestCodeIdAsync(testCode.id);

            foreach (var testExpectIO in testExpectIOs)
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