using speedCode.Core.judge0.dto;

namespace speedCode.codeTesting;

public abstract class JudgeCode
{
    public abstract string ExpectedResult { get; set; }
    public abstract string OutPut { get; set; }
    public abstract string Input { get; set; }
    public abstract JudgeCodeTestResults TestOutput();
}