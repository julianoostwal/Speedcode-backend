using System.Text.RegularExpressions;
using speedCode.codeTesting.attributes;
using speedCode.codeTesting.enums;
using speedCode.dto;

namespace speedCode.codeTesting;

[JudgeTypesAttributes(TestType.Regex)]
public class RegexJudgeCode : JudgeCode
{
    public override required string[] ExpectedResults { get; set; }
    public override required string OutPut { get; set; }

    public override JudgeCodeTestResults TestOutput()
    {
        JudgeCodeTestResults results = new JudgeCodeTestResults();
        foreach (var expectedResult in ExpectedResults)
        {
            Match match = Regex.Match(OutPut, expectedResult);
            results.Logs.Add(new JudgeCodeTestLog
            {
                Regex = expectedResult,
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
        }

        return results;
    }
}