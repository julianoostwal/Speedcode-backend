using speedCode.Profile.model;
using speedCode.Profile.Model;

namespace speedCode.Profile.Interfaces;

public interface IProfileRepository
{
    Task<Profiles?> GetByIdAsync(Guid id);
    Task<List<Profiles>> GetAllAsync();
    Task<Profiles> CreateAsync(Profiles profile);
    Task<Profiles> UpdateAsync(Profiles profile);
    Task<bool> DeleteAsync(Guid id);
    
    // Achievement operations
    Task<Achievement?> GetAchievementByIdAsync(Guid id);
    Task<List<Achievement>> GetAllAchievementsAsync();
    Task<Achievement> CreateAchievementAsync(Achievement achievement);
    
    // CompletedProfileExercise operations
    Task<List<CompletedProfileExercise>> GetCompletedExercisesByProfileIdAsync(Guid profileId);
    Task<CompletedProfileExercise> CreateCompletedExerciseAsync(CompletedProfileExercise completedExercise);
    Task<List<Guid>> GetCompletedExerciseIdsByProfileIdAsync(Guid profileId);
    
    // CreatedProfileExercises operations
    Task<List<CreatedProfileExercises>> GetCreatedExercisesByProfileIdAsync(Guid profileId);
    Task<CreatedProfileExercises> CreateCreatedExerciseAsync(CreatedProfileExercises createdExercise);
    Task<List<Guid>> GetCreatedExerciseIdsByProfileIdAsync(Guid profileId);
    
    // ProfileAchivement operations
    Task<List<ProfileAchivement>> GetProfileAchievementsByProfileIdAsync(Guid profileId);
    Task<ProfileAchivement> CreateProfileAchievementAsync(ProfileAchivement profileAchivement);
}
