using speedCode.codeTesting.attributes;
using speedCode.codeTesting.enums;
using speedCode.dto;

namespace speedCode.codeTesting;

[JudgeTypesAttributes(TestType.Exactly)]
public class ExactJudgeCode : JudgeCode
{
    public override string ExpectedResult { get; set; }
    public override string OutPut { get; set; }

    public override JudgeCodeTestResults TestOutput()
    {
        var results = new JudgeCodeTestResults()
        {
            Status = JudgeCodeTestStatus.Passed
        };

        bool match = string.Equals(OutPut, ExpectedResult, StringComparison.Ordinal);

        results.Logs.Add(new JudgeCodeTestLog
        {
            outPut = OutPut,
            Regex = ExpectedResult,
            TestType = TestType.Exactly,
            Accepted = match
        });

        if (!match)
        {
            results.Status = JudgeCodeTestStatus.NotPassed;
        }

        return results;
    }
}