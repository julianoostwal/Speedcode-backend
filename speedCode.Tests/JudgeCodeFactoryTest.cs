using Xunit;
using speedCode.codeTesting;
using speedCode.codeTesting.attributes;
using speedCode.dto;

namespace speedCode.tests
{
    //mock class
    [JudgeTypesAttributes(TestType.Exactly)]
    public class ExactJudgeCode : JudgeCode
    {
        public override string[] ExpectedResults { get; set; }
        public override string OutPut { get; set; }

        public override JudgeCodeTestResults TestOutput()
        {
            throw new NotImplementedException();
        }
    }


    public class JudgeCodeFactoryTests
    {
        [Fact]
        public void CreateJudgeCode_ShouldReturnExactJudgeCode_WhenExactTestTypeGiven()
        {
            // Arrange
            var testType = TestType.Exactly;

            // Act
            var judgeCode = JudgeCodeFactory.CreateJudgeCode(testType);

            // Assert
            Assert.NotNull(judgeCode);
            Assert.IsAssignableFrom<JudgeCode>(judgeCode);
        }

        [Fact]
        public void CreateJudgeCode_ShouldThrowArgumentException_WhenTestTypeNotMapped()
        {
            // Arrange
            var invalidType = (TestType)999;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                JudgeCodeFactory.CreateJudgeCode(invalidType)
            );

            Assert.Contains("No JudgeCode class found", exception.Message);
        }
    }
}