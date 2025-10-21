using speedCode.Core.judge0.dto;
using speedCode.Core.judge0.enums;
using speedCode.dto;
using speedCode.Judge0.attributes;

namespace speedCode.codeTesting;

[JudgeTypesAttributes(TestType.Contains)]
public class ContainJudgeCode : JudgeCode
{
    public override required string ExpectedResult { get; set; }

    public override required SubmissionResult SubmissionResult { get; set; }

    public override JudgeCodeTestResults TestOutput()
    {
        JudgeCodeTestResults results = new JudgeCodeTestResults()
        {
            Status = JudgeCodeTestStatus.Passed
        };


        bool contains = SubmissionResult.Result.Stdout?.Contains(ExpectedResult, StringComparison.OrdinalIgnoreCase) ??
                        false;


        results.Logs.Add(new JudgeCodeTestLog
        {
            input = SubmissionResult.Stdin,
            outPut = SubmissionResult.Result.Stdout,
            TestType = TestType.Contains,
            Regex = ExpectedResult,
            Accepted = contains
        });

        if (!contains)
        {
            results.Status = JudgeCodeTestStatus.NotPassed;
        }

        return results;
    }
}