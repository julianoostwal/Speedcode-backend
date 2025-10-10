using speedCode.codeTesting;
using speedCode.codeTesting.enums;

namespace speedCode.dto;

public class TestCodeAsyncResults
{
    public List<JudgeCodeTestResults> TestProcess { get; set; } = new List<JudgeCodeTestResults>();

    public bool IsPassed => TestProcess.All(tp => tp.Status == JudgeCodeTestStatus.Passed);
}