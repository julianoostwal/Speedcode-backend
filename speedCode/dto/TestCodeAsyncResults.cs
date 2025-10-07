using speedCode.codeTesting;

namespace speedCode.dto;

public class TestCodeAsyncResults
{
    public List<JudgeCodeTestResults> TestProcess { get; set; } = new List<JudgeCodeTestResults>();

    public bool IsPassed { get; set; }
}