namespace speedCode.codeTesting;

public class RegexJudgeCode : JudgeCode
{
    public string[] ExpectedResults { get; set; }
    public string CheckType { get; set; }
    public string Code { get; set; }

    public override JudgeCodeTestResults TestCode(string code, string[] expectedResults)
    {
        throw new NotImplementedException();
    }
}