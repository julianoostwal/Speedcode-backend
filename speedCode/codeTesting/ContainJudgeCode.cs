using speedCode.codeTesting.attributes;
using speedCode.codeTesting.enums;
using speedCode.dto;

namespace speedCode.codeTesting;

[JudgeTypesAttributes(TestType.Contains)]
public class ContainJudgeCode : JudgeCode
{
    public override required string[] ExpectedResults { get; set; }
    public override required string OutPut { get; set; }

    public override JudgeCodeTestResults TestOutput()
    {
        JudgeCodeTestResults results = new JudgeCodeTestResults()
        {
            Status = JudgeCodeTestStatus.Passed
        };

        foreach (var expected in ExpectedResults)
        {
            bool contains = OutPut?.Contains(expected, StringComparison.OrdinalIgnoreCase) ?? false;


            results.Logs.Add(new JudgeCodeTestLog
            {
                Regex = expected,
                Accepted = contains
            });

            if (!contains)
            {
                results.Status = JudgeCodeTestStatus.NotPassed;
            }
        }


        return results;
    }
}