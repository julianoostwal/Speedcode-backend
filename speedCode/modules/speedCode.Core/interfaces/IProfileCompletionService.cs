namespace speedCode.Core.interfaces;

public interface IProfileCompletionService
{
    Task SaveCompletedExerciseAsync(Guid profileId, Guid exerciseId, double memory, TimeSpan time);
}
