using speedCode.Exercise.model;

namespace speedCode.Exercise.Interfaces;

public interface IExerciseRepository
{
    Task<model.Exercise?> GetByIdAsync(Guid id);
    Task<List<model.Exercise>> GetAllAsync();
    Task<List<model.Exercise>> GetRandomExercisesAsync(int count);
    Task<model.Exercise> CreateAsync(model.Exercise exercise);
    Task<model.Exercise> UpdateAsync(model.Exercise exercise);
    Task<bool> DeleteAsync(Guid id);
    
    // TestCode operations
    Task<TestCode?> GetTestCodeByIdAsync(Guid id);
    Task<List<TestCode>> GetTestCodesByExerciseIdAsync(Guid exerciseId);
    Task<TestCode> CreateTestCodeAsync(TestCode testCode);
    
    // TestExpectIO operations
    Task<List<TestExpectIO>> GetTestExpectIOsByTestCodeIdAsync(Guid testCodeId);
    Task<TestExpectIO> CreateTestExpectIOAsync(TestExpectIO testExpectIO);
    
    // ExerciseTest operations
    Task<List<ExerciseTest>> GetExerciseTestsByExerciseIdAsync(Guid exerciseId);
    Task<ExerciseTest> CreateExerciseTestAsync(ExerciseTest exerciseTest);
}
