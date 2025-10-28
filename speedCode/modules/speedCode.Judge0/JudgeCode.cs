using speedCode.Core.judge0.dto;
using speedCode.dto;

namespace speedCode.codeTesting;

public abstract class JudgeCode
{
    public abstract string ExpectedResult { get; set; }
    public abstract SubmissionResult SubmissionResult { get; set; }
    public abstract JudgeCodeTestResults TestOutput();
}