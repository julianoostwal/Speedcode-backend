using Microsoft.EntityFrameworkCore;
using speedCode.codeTesting;
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

    public async void TestCodeAsync()
    {
        
        
        
        Task<SubmissionResult> result = _judge0Client.SubmitCodeAsync("print('Hello World')", 70);
        
        JudgeCodeFactory.CreateJudgeCode(TestType.Regex);
        Console.WriteLine(await result);
    }
}