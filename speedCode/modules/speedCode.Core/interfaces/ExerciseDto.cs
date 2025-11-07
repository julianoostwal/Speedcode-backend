namespace speedCode.Core.interfaces;

public class ExerciseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsPrivate { get; set; }
    public Guid? CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Difficulty { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
}

