using Microsoft.EntityFrameworkCore;
using speedCode.codeTesting;
using speedCode.codeTesting.enums;
using speedCode.Data;
using speedCode.dto;
using speedCode.Model;

namespace speedCode.services;

public class JudgeCodeService
{
    private readonly Judge0Client _judge0Client;
    private readonly AppDbContext _dbContext;

    public JudgeCodeService(Judge0Client judge0Client, AppDbContext dbContext)
    {
        _judge0Client = judge0Client;
        _dbContext = dbContext;
    }

    public async Task<TestCodeAsyncResults> TestCodeAsync(TestCodeRequest request)
    {
        TestCodeAsyncResults results = new TestCodeAsyncResults();
        List<TestCode> testCodes = await _dbContext.Exercise
            .Include(e => e.ExerciseTests)
            .ThenInclude(et => et.test)
            .Where(e => e.id == request.ExerciseId)
            .SelectMany(e => e.ExerciseTests.Select(et => et.test))
            .ToListAsync();

        foreach (var testCode in testCodes)
        {
            JudgeCode judgeCodeFactoryResults = JudgeCodeFactory.CreateJudgeCode(testCode.Type);
            judgeCodeFactoryResults.ExpectedResults = testCode.expectedResult;
            judgeCodeFactoryResults.OutPut =
                _judge0Client.SubmitCodeAsync(request.Code, request.LanguageId).Result.Stdout;
            JudgeCodeTestResults output = judgeCodeFactoryResults.TestOutput();
            results.TestProcess.Add(output);
        }

        foreach (var proces in results.TestProcess)
        {
            if (proces.Status != JudgeCodeTestStatus.Passed)
            {
                results.IsPassed = false;
            }
        }

        return await Task.FromResult(results);
    }
}