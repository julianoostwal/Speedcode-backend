using speedCode.Core.interfaces;
using speedCode.Profile.Interfaces;
using speedCode.Profile.model;
using speedCode.Profile.Model;

namespace speedCode.Profile.service;

public class ProfileCompletionService : IProfileCompletionService
{
    private readonly IProfileRepository _profileRepository;

    public ProfileCompletionService(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task SaveCompletedExerciseAsync(Guid profileId, Guid exerciseId, double memory, TimeSpan time)
    {
        var completedExercise = new CompletedProfileExercise
        {
            profileId = profileId,
            exerciseId = exerciseId,
            memory = memory,
            time = time
        };
        
        await _profileRepository.CreateCompletedExerciseAsync(completedExercise);
    }
}

