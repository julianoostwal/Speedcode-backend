using speedCode.Core.judge0.dto;

namespace speedCode.Core.interfaces;

public interface IExerciseTestService
{
    Task<TestCodeAsyncResults> TestCodeAsync(TestCodeRequest request);
}

