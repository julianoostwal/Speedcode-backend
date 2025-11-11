namespace speedCode.Core.interfaces;

public class CompletedProfileExerciseDto
{
    public Guid profileId { get; set; }
    public Guid exerciseId { get; set; }
    public TimeSpan time { get; set; }
}