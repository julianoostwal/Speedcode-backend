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
                Input = "hello",
                ExpectedResult = "Hello World from speedCode"
            };

            var result = judgeCode.TestOutput();
            Assert.Equal(JudgeCodeTestStatus.NotPassed, result.Status);
        }


        [Fact]
        public void TestJudgeCodeTestResultsWithRegex_ShouldReturnTrue_WhenPatternMatches()
        {
            // Arrange
            var judgeCode = new RegexJudgeCode
            {
                OutPut = "Hello World from speedCode",
                Input = "hello",
                ExpectedResult = "Hello World from speedCode"
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
            var judgeCode = new RegexJudgeCode
            {
                OutPut = "Hello World from speedCode",
                Input = "hello",
                ExpectedResult = "Hello World from speedCode123"
            };
            // Act
            JudgeCodeTestResults result = judgeCode.TestOutput();

            // Assert
            Assert.Equal(JudgeCodeTestStatus.NotPassed, result.Status);
        }
    }
}