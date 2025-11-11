using speedCode.Core.interfaces;
using speedCode.Profile.Interfaces;
using speedCode.Profile.model;

namespace speedCode.Profile.service;

public class ProfileQueryService : IProfileQueryService
{
    private readonly IProfileRepository _profileRepository;

    public ProfileQueryService(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<ProfileDto?> GetByIdAsync(Guid id)
    {
        var profile = await _profileRepository.GetByIdAsync(id);
        return profile != null ? MapToDto(profile) : null;
    }

    public async Task<List<ProfileDto>> GetByIdsAsync(List<Guid> ids)
    {
        var profiles = new List<ProfileDto>();
        foreach (var id in ids)
        {
            var profile = await _profileRepository.GetByIdAsync(id);
            if (profile != null)
            {
                profiles.Add(MapToDto(profile));
            }
        }
        return profiles;
    }

    private static ProfileDto MapToDto(Model.Profiles profile)
    {
        return new ProfileDto
        {
            id = profile.id,
            avatarUrl = profile.avatarUrl,
            nickName = profile.nickName,
            role = profile.role.ToString(),
            createdAt = profile.createdAt
        };
    }

    public async Task<List<CompletedProfileExerciseDto>> GetCompletedProfileExercisesAsync(List<Guid> profileIds, List<Guid> exerciseIds)
    {
        var allCompleted = new List<CompletedProfileExercise>();
        foreach (var profileId in profileIds)
        {
            var completed = await _profileRepository.GetCompletedExercisesByProfileIdAsync(profileId);
            if (completed != null)
                allCompleted.AddRange(completed);
        }

        var filtered = allCompleted
            .Where(cpe => exerciseIds.Contains(cpe.exerciseId))
            .ToList();

        return filtered.Select(cpe => new CompletedProfileExerciseDto
        {
            profileId = cpe.profileId,
            exerciseId = cpe.exerciseId,
            time = cpe.time
        }).ToList();
    }
}

