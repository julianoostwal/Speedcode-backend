using speedCode.Core.judge0.dto;
using speedCode.Core.judge0.enums;

using speedCode.Judge0.attributes;

namespace speedCode.Judge0;

[JudgeTypesAttributes(TestType.Exactly)]
public class ExactJudgeCode : JudgeCode
{
    public override string ExpectedResult { get; set; }
    public override required SubmissionResult SubmissionResult { get; set; }

    public override JudgeCodeTestResults TestOutput()
    {
        var results = new JudgeCodeTestResults()
        {
            Status = JudgeCodeTestStatus.Passed
        };

        bool match = string.Equals(SubmissionResult.Result.Stdout, ExpectedResult, StringComparison.Ordinal);

        results.Logs.Add(new JudgeCodeTestLog
        {
            input = SubmissionResult.Stdin,
            result = SubmissionResult.Result,
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