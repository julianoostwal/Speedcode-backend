using speedCode.Core.judge0.dto;
namespace speedCode.Judge0;

public abstract class JudgeCode
{
    public abstract string ExpectedResult { get; set; }
    public abstract SubmissionResult SubmissionResult { get; set; }
    public abstract JudgeCodeTestResults TestOutput();
}