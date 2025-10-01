using speedCode.codeTesting;
using speedCode.codeTesting.enums;

namespace speedCode.Tests
{
    public class RegexJudgeCodeTests
    {
        [Fact]
        public void TestRegexJudgeCode_ShouldHandleEmptyOutput()
        {
            var judgeCode = new RegexJudgeCode
            {
                OutPut = "",
                ExpectedResults = new string[] { @"^Hello$" }
            };

            var result = judgeCode.TestOutput();
            Assert.Equal(JudgeCodeTestStatus.NotPassed, result.Status);
        }


        [Fact]
        public void TestJudgeCodeTestResultsWithRegex_ShouldReturnTrue_WhenPatternMatches()
        {
            // Arrange
            var judgeCode = new RegexJudgeCode()
            {
                ExpectedResults = new string[] { @"^Hello World$" },
                OutPut = "Hello World"
            };

            // Act
            JudgeCodeTestResults result = judgeCode.TestOutput();
            // Assert
            Assert.Equal(JudgeCodeTestStatus.Passed, result.Status);
        }

        [Fact]
        public void TestJudgeCodeTestResultsWithRegex_ShouldReturnFalse_WhenPatternDoesNotMatch()
        {
            // Arrange
            var judgeCode = new RegexJudgeCode()
            {
                ExpectedResults = new string[] { @"^Hello World$" },
                OutPut = "Hello Worldss"
            };

            // Act
            JudgeCodeTestResults result = judgeCode.TestOutput();

            // Assert
            Assert.Equal(JudgeCodeTestStatus.NotPassed, result.Status);
        }
    }
}