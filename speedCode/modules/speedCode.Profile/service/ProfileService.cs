using speedCode.exercise;
using speedCode.exercise.model;

namespace speedCode.Profile.service;

public class ProfileService
{
    private readonly ProfileDb _profileDb;
    private readonly exerciseDb _dbContext;

    public List<Exercise> GetAllCompletedexercisesByProfileId(Guid profileId)
    {
        return (from e in _dbContext.Exercise
            join pe in _profileDb.CompletedProfileExercise
                on e.id equals pe.exerciseId into peGroup
            from pe in peGroup.DefaultIfEmpty()
            where pe.profileId == profileId
            select e).ToList();
    }

    public List<Exercise> GetAllCreatedAndPublicExercisesByProfileId(Guid profileId)
    {
        return (
            from e in _dbContext.Exercise
            join pe in _profileDb.CreatedProfileExercise
                on e.id equals pe.exerciseId into peGroup
            from pe in peGroup.DefaultIfEmpty()
            where (pe != null && pe.profileId == profileId) || e.isPrivate == false
            select e
        ).ToList();
    }
}