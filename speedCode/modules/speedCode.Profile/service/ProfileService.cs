using speedCode.exercise;

namespace speedCode.Profile.service;

public class ProfileService
{
    private readonly ProfileDb _profileDb;
    private readonly ExerciseDb _dbContext;

    public ProfileService(ProfileDb profileDb, ExerciseDb dbContext)
    {
        _profileDb = profileDb;
        _dbContext = dbContext;
    }


    public List<Exercise.model.Exercise> GetAllCompletedExercisesByProfileId(Guid profileId)
    {
        var completedIds = _profileDb.CompletedProfileExercise
            .Where(p => p.profileId == profileId)
            .Select(p => p.exerciseId)
            .ToList();

        return _dbContext.Exercise
            .Where(e => completedIds.Contains(e.id))
            .ToList();
    }

    public List<Exercise.model.Exercise> GetAllCreatedAndPublicExercisesByProfileId(Guid profileId)
    {
        var createdIds = _profileDb.CreatedProfileExercise
            .Where(p => p.profileId == profileId)
            .Select(p => p.exerciseId)
            .ToList();

        return _dbContext.Exercise
            .Where(e => !e.isPrivate || createdIds.Contains(e.id))
            .ToList();
    }
}