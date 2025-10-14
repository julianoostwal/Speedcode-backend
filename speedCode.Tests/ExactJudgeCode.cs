using Xunit;
using speedCode.codeTesting;
using speedCode.codeTesting.enums;

namespace speedCode.Tests
{
    public class ExactJudgeCodeTests
    {
        [Fact]
        public void TestExactJudgeCode_ShouldPass_WhenOutputContainsExpected()
        {
            var judgeCode = new ExactJudgeCode
            {
                OutPut = "Hello World from speedCode",
                Input = "hello",
                ExpectedResult = "Hello World from speedCode"
            };

            var result = judgeCode.TestOutput();

            Assert.Equal(JudgeCodeTestStatus.Passed, result.Status);
            Assert.All(result.Logs, log => Assert.True(log.Accepted));
        }

        [Fact]
        public void TestExactJudgeCode_ShouldFail_WhenOutputMissingExpected()
        {
            
            var judgeCode = new ExactJudgeCode
            {
                OutPut = "Hello World from speedCode",
                Input = "hello",
                ExpectedResult = "Hello World from speedCode123"
            };

            var result = judgeCode.TestOutput();

            Assert.Equal(JudgeCodeTestStatus.NotPassed, result.Status);
            Assert.Contains(result.Logs, log => !log.Accepted);
        }
    }
}