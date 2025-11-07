using speedCode.Core.interfaces;
using speedCode.Core.judge0.dto;

namespace speedCode.Exercise.service;

public class ExerciseTestService : IExerciseTestService
{
    private readonly ExerciseService _exerciseService;

    public ExerciseTestService(ExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    public async Task<TestCodeAsyncResults> TestCodeAsync(TestCodeRequest request)
    {
        return await _exerciseService.TestCodeAsync(request);
    }
}

