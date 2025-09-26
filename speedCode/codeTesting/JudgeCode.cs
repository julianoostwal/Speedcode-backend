namespace speedCode.codeTesting;

public abstract class JudgeCode
{
    public abstract string[] ExpectedResults { get; set; }
    public abstract string OutPut { get; set; }
    public abstract JudgeCodeTestResults TestOutput();
}