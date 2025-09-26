using Microsoft.EntityFrameworkCore;
using speedCode.codeTesting;
using speedCode.Data;
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

    public async void TestCodeAsync(string code, Guid exerciseId, int languageId)
    {
        List<TestCode> testCodes = await _dbContext.ExerciseTest
            .Where(et => et.exerciseId == exerciseId)
            .Include(et => et.test)
            .Select(et => et.test)
            .ToListAsync();

        Console.WriteLine(testCodes);
    }
}