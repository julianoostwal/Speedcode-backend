using System.Text.RegularExpressions;
using speedCode.codeTesting.enums;
using speedCode.Utils;

namespace speedCode.codeTesting;

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
            results.Log = new JudgeCodeTestLog
            {
                Regex = expectedResult,
                Accepted = match.Success
            };

            if (!match.Success)
            {
                results.Status = JudgeCodeTestStatus.NotPassed;
            }
        }

        return results;
    }
}