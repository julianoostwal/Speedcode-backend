using speedCode.Core.interfaces;
using speedCode.Exercise.Interfaces;

namespace speedCode.Exercise.service;

public class ExerciseQueryService : IExerciseQueryService
{
    
    private readonly IExerciseRepository _exerciseRepository;

    public ExerciseQueryService(IExerciseRepository exerciseRepository)
    {
        _exerciseRepository = exerciseRepository;
    }

    public async Task<ExerciseDto?> GetByIdAsync(Guid id)
    {
        var exercise = await _exerciseRepository.GetByIdAsync(id);
        return exercise != null ? MapToDto(exercise) : null;
    }

    public async Task<List<ExerciseDto>> GetAllAsync()
    {
        var exercises = await _exerciseRepository.GetAllAsync();
        return exercises.Select(MapToDto).ToList();
    }

    public async Task<List<ExerciseDto>> GetByIdsAsync(List<Guid> ids)
    {
        var exercises = new List<ExerciseDto>();
        foreach (var id in ids)
        {
            var exercise = await _exerciseRepository.GetByIdAsync(id);
            if (exercise != null)
            {
                exercises.Add(MapToDto(exercise));
            }
        }
        return exercises;
    }

    public async Task<List<ExerciseDto>> GetPublicExercisesAsync()
    {
        var allExercises = await _exerciseRepository.GetAllAsync();
        return allExercises
            .Where(e => !e.isPrivate)
            .Select(MapToDto)
            .ToList();
    }

    private static ExerciseDto MapToDto(model.Exercise exercise)
    {
        return new ExerciseDto
        {
            Points = exercise.points,
            Id = exercise.id,
            Title = exercise.title,
            Description = exercise.description,
            IsPrivate = exercise.isPrivate,
            CreatedAt = exercise.createdAt,
            Difficulty = exercise.Level.ToString(),
            Language = ""
        };
    }
}
