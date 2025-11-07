using speedCode.Core.interfaces;
using speedCode.Profile.Interfaces;
using speedCode.Profile.Model;

namespace speedCode.Profile.service;

public class ProfileService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IExerciseQueryService _exerciseQueryService;

    public ProfileService(IProfileRepository profileRepository, IExerciseQueryService exerciseQueryService)
    {
        _profileRepository = profileRepository;
        _exerciseQueryService = exerciseQueryService;
    }

    public Profiles? GetProfileById(Guid id)
    {
        return _profileRepository.GetByIdAsync(id).Result;
    }

    public async Task<List<ExerciseDto>> GetAllCompletedExercisesByProfileIdAsync(Guid profileId)
    {
        var completedIds = await _profileRepository.GetCompletedExerciseIdsByProfileIdAsync(profileId);
        return await _exerciseQueryService.GetByIdsAsync(completedIds);
    }

    public async Task<List<ExerciseDto>> GetAllCreatedAndPublicExercisesByProfileIdAsync(Guid profileId)
    {
        var createdIds = await _profileRepository.GetCreatedExerciseIdsByProfileIdAsync(profileId);
        var allExercises = await _exerciseQueryService.GetAllAsync();
        
        return allExercises
            .Where(e => !e.IsPrivate || createdIds.Contains(e.Id))
            .ToList();
    }
}