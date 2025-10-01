using Xunit;
using speedCode.codeTesting;
using speedCode.codeTesting.enums;

namespace speedCode.Tests
{
    public class ContainJudgeCodeTests
    {
        [Fact]
        public void TestContainJudgeCode_ShouldPass_WhenOutputContainsExpected()
        {
            var judgeCode = new ContainJudgeCode
            {
                OutPut = "Hello World from speedCode",
                ExpectedResults = new string[] { "Hello", "speedCode" }
            };

            var result = judgeCode.TestOutput();

            Assert.Equal(JudgeCodeTestStatus.Passed, result.Status);
            Assert.All(result.Logs, log => Assert.True(log.Accepted));
        }

        [Fact]
        public void TestContainJudgeCode_ShouldFail_WhenOutputMissingExpected()
        {
            var judgeCode = new ContainJudgeCode
            {
                OutPut = "Hello World",
                ExpectedResults = new string[] { "Hello", "speedCode" }
            };

            var result = judgeCode.TestOutput();

            Assert.Equal(JudgeCodeTestStatus.NotPassed, result.Status);
            Assert.Contains(result.Logs, log => !log.Accepted);
        }
    }
}