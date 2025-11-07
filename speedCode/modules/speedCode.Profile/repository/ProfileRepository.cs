using Microsoft.EntityFrameworkCore;
using speedCode.Profile.Interfaces;
using speedCode.Profile.model;
using speedCode.Profile.Model;

namespace speedCode.Profile.repository;

public class ProfileRepository : IProfileRepository
{
    private readonly ProfileDb _dbContext;

    public ProfileRepository(ProfileDb dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Profiles?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Profiles.FindAsync(id);
    }

    public async Task<List<Profiles>> GetAllAsync()
    {
        return await _dbContext.Profiles.ToListAsync();
    }

    public async Task<Profiles> CreateAsync(Profiles profile)
    {
        _dbContext.Profiles.Add(profile);
        await _dbContext.SaveChangesAsync();
        return profile;
    }

    public async Task<Profiles> UpdateAsync(Profiles profile)
    {
        _dbContext.Profiles.Update(profile);
        await _dbContext.SaveChangesAsync();
        return profile;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var profile = await _dbContext.Profiles.FindAsync(id);
        if (profile == null) return false;
        
        _dbContext.Profiles.Remove(profile);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Achievement?> GetAchievementByIdAsync(Guid id)
    {
        return await _dbContext.Achievement.FindAsync(id);
    }

    public async Task<List<Achievement>> GetAllAchievementsAsync()
    {
        return await _dbContext.Achievement.ToListAsync();
    }

    public async Task<Achievement> CreateAchievementAsync(Achievement achievement)
    {
        _dbContext.Achievement.Add(achievement);
        await _dbContext.SaveChangesAsync();
        return achievement;
    }

    public async Task<List<CompletedProfileExercise>> GetCompletedExercisesByProfileIdAsync(Guid profileId)
    {
        return await _dbContext.CompletedProfileExercise
            .Where(pe => pe.profileId == profileId)
            .ToListAsync();
    }

    public async Task<CompletedProfileExercise> CreateCompletedExerciseAsync(CompletedProfileExercise completedExercise)
    {
        _dbContext.CompletedProfileExercise.Add(completedExercise);
        await _dbContext.SaveChangesAsync();
        return completedExercise;
    }

    public async Task<List<Guid>> GetCompletedExerciseIdsByProfileIdAsync(Guid profileId)
    {
        return await _dbContext.CompletedProfileExercise
            .Where(pe => pe.profileId == profileId)
            .Select(pe => pe.exerciseId)
            .ToListAsync();
    }

    public async Task<List<CreatedProfileExercises>> GetCreatedExercisesByProfileIdAsync(Guid profileId)
    {
        return await _dbContext.CreatedProfileExercise
            .Where(pe => pe.profileId == profileId)
            .ToListAsync();
    }

    public async Task<CreatedProfileExercises> CreateCreatedExerciseAsync(CreatedProfileExercises createdExercise)
    {
        _dbContext.CreatedProfileExercise.Add(createdExercise);
        await _dbContext.SaveChangesAsync();
        return createdExercise;
    }

    public async Task<List<Guid>> GetCreatedExerciseIdsByProfileIdAsync(Guid profileId)
    {
        return await _dbContext.CreatedProfileExercise
            .Where(pe => pe.profileId == profileId)
            .Select(pe => pe.exerciseId)
            .ToListAsync();
    }

    public async Task<List<ProfileAchivement>> GetProfileAchievementsByProfileIdAsync(Guid profileId)
    {
        return await _dbContext.ProfileAchievement
            .Where(pa => pa.profileId == profileId)
            .ToListAsync();
    }

    public async Task<ProfileAchivement> CreateProfileAchievementAsync(ProfileAchivement profileAchivement)
    {
        _dbContext.ProfileAchievement.Add(profileAchivement);
        await _dbContext.SaveChangesAsync();
        return profileAchivement;
    }
}
