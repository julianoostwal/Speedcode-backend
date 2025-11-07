using Microsoft.EntityFrameworkCore;
using speedCode.exercise;
using speedCode.Exercise.Interfaces;
using speedCode.Exercise.model;

namespace speedCode.Exercise.repository;

public class ExerciseRepository : IExerciseRepository
{
    private readonly ExerciseDb _dbContext;

    public ExerciseRepository(ExerciseDb dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<model.Exercise?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Exercise.FindAsync(id);
    }

    public async Task<List<model.Exercise>> GetAllAsync()
    {
        return await _dbContext.Exercise.ToListAsync();
    }

    public async Task<List<model.Exercise>> GetRandomExercisesAsync(int count)
    {
        return await _dbContext.Exercise
            .OrderBy(e => EF.Functions.Random())
            .Take(count)
            .ToListAsync();
    }

    public async Task<model.Exercise> CreateAsync(model.Exercise exercise)
    {
        _dbContext.Exercise.Add(exercise);
        await _dbContext.SaveChangesAsync();
        return exercise;
    }

    public async Task<model.Exercise> UpdateAsync(model.Exercise exercise)
    {
        _dbContext.Exercise.Update(exercise);
        await _dbContext.SaveChangesAsync();
        return exercise;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var exercise = await _dbContext.Exercise.FindAsync(id);
        if (exercise == null) return false;
        
        _dbContext.Exercise.Remove(exercise);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<TestCode?> GetTestCodeByIdAsync(Guid id)
    {
        return await _dbContext.TestCode.FindAsync(id);
    }

    public async Task<List<TestCode>> GetTestCodesByExerciseIdAsync(Guid exerciseId)
    {
        return await (from et in _dbContext.ExerciseTest
                     join tc in _dbContext.TestCode on et.testId equals tc.id
                     where et.exerciseId == exerciseId
                     select tc)
                    .Distinct()
                    .ToListAsync();
    }

    public async Task<TestCode> CreateTestCodeAsync(TestCode testCode)
    {
        _dbContext.TestCode.Add(testCode);
        await _dbContext.SaveChangesAsync();
        return testCode;
    }

    public async Task<List<TestExpectIO>> GetTestExpectIOsByTestCodeIdAsync(Guid testCodeId)
    {
        return await _dbContext.TestExpectIos
            .Where(io => io.testCodeId == testCodeId)
            .ToListAsync();
    }

    public async Task<TestExpectIO> CreateTestExpectIOAsync(TestExpectIO testExpectIO)
    {
        _dbContext.TestExpectIos.Add(testExpectIO);
        await _dbContext.SaveChangesAsync();
        return testExpectIO;
    }

    public async Task<List<ExerciseTest>> GetExerciseTestsByExerciseIdAsync(Guid exerciseId)
    {
        return await _dbContext.ExerciseTest
            .Where(et => et.exerciseId == exerciseId)
            .ToListAsync();
    }

    public async Task<ExerciseTest> CreateExerciseTestAsync(ExerciseTest exerciseTest)
    {
        _dbContext.ExerciseTest.Add(exerciseTest);
        await _dbContext.SaveChangesAsync();
        return exerciseTest;
    }
}
