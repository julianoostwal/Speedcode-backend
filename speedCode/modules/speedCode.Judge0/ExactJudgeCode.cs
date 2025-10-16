using speedCode.Core.judge0.dto;
using speedCode.Core.judge0.enums;
using speedCode.dto;
using speedCode.Judge0.attributes;

namespace speedCode.codeTesting;

[JudgeTypesAttributes(TestType.Exactly)]
public class ExactJudgeCode : JudgeCode
{
    public override string ExpectedResult { get; set; }
    public override string OutPut { get; set; }
    
    public override string Input { get; set; }
    public override JudgeCodeTestResults TestOutput()
    {
        var results = new JudgeCodeTestResults()
        {
            Status = JudgeCodeTestStatus.Passed
        };

        bool match = string.Equals(OutPut, ExpectedResult, StringComparison.Ordinal);

        results.Logs.Add(new JudgeCodeTestLog
        {
            input = Input,
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