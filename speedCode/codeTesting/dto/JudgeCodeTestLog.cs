using speedCode.dto;

namespace speedCode.codeTesting;

public class JudgeCodeTestLog
{
    public required string Regex { get; set; }
    public required TestType TestType { get; set; }
    public required bool Accepted { get; set; }
}