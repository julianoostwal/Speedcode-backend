using speedCode.Core.judge0.enums;

namespace speedCode.Core.judge0.dto;
public class TestCodeAsyncResults
{
    public List<JudgeCodeTestResults> TestProcess { get; set; } = new List<JudgeCodeTestResults>();

    public bool IsPassed => TestProcess.All(tp => tp.Status == JudgeCodeTestStatus.Passed);
}