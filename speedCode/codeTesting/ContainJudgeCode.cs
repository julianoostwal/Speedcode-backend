using speedCode.codeTesting.attributes;
using speedCode.codeTesting.enums;
using speedCode.dto;

namespace speedCode.codeTesting;

[JudgeTypesAttributes(TestType.Contains)]
public class ContainJudgeCode : JudgeCode
{
    public override required string ExpectedResult { get; set; }

    public override required string OutPut { get; set; }

    public override string Input { get; set; }

    public override JudgeCodeTestResults TestOutput()
    {
        JudgeCodeTestResults results = new JudgeCodeTestResults()
        {
            Status = JudgeCodeTestStatus.Passed
        };


        bool contains = OutPut?.Contains(ExpectedResult, StringComparison.OrdinalIgnoreCase) ?? false;


        results.Logs.Add(new JudgeCodeTestLog
        {
            input = Input,
            outPut = OutPut,
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