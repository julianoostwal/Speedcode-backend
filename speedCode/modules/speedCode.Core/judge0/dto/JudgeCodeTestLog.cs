namespace speedCode.Core.judge0.dto;

public class JudgeCodeTestLog
{
    public required string Regex { get; set; }
    public required TestType TestType { get; set; }

    public required string input { get; set; }
    
    public required Judge0Response result { get; set; }
    public required bool Accepted { get; set; }
}