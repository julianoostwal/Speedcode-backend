using System.Text.RegularExpressions;
using speedCode.Core.judge0.dto;
using speedCode.Core.judge0.enums;
using speedCode.dto;
using speedCode.Judge0.attributes;

namespace speedCode.codeTesting;

[JudgeTypesAttributes(TestType.Regex)]
public class RegexJudgeCode : JudgeCode
{
    public override required string ExpectedResult { get; set; }
    public override required string OutPut { get; set; }

    public override string Input { get; set; }

    public override JudgeCodeTestResults TestOutput()
    {
        JudgeCodeTestResults results = new JudgeCodeTestResults();

        Match match = Regex.Match(OutPut, ExpectedResult);
        results.Logs.Add(new JudgeCodeTestLog
        {
            input = Input,
            outPut = OutPut,
            Regex = ExpectedResult,
            TestType = TestType.Regex,
            Accepted = match.Success
        });


        if (match.Success)
        {
            results.Status = JudgeCodeTestStatus.Passed;
        }
        else
        {
            results.Status = JudgeCodeTestStatus.NotPassed;
        }

        return results;
    }
}