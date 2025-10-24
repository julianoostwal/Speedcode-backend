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
    public override required SubmissionResult SubmissionResult { get; set; }


    public override JudgeCodeTestResults TestOutput()
    {
        JudgeCodeTestResults results = new JudgeCodeTestResults();

        Match match = Regex.Match(SubmissionResult.Result.Stdout, ExpectedResult);
        results.Logs.Add(new JudgeCodeTestLog
        {
            input = SubmissionResult.Stdin,
            result = SubmissionResult.Result,
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