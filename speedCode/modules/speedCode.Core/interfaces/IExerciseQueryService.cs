namespace speedCode.Core.interfaces;

public interface IExerciseQueryService
{
    Task<ExerciseDto?> GetByIdAsync(Guid id);
    Task<List<ExerciseDto>> GetAllAsync();
    Task<List<ExerciseDto>> GetByIdsAsync(List<Guid> ids);
    Task<List<ExerciseDto>> GetPublicExercisesAsync();
}
