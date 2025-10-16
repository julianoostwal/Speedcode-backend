using speedCode.Core.judge0.dto;

namespace speedCode.Judge0.attributes;

[AttributeUsage(
    AttributeTargets.Class,
    Inherited = false,
    AllowMultiple = false
)]
public class JudgeTypesAttributes : Attribute
{
    public TestType TestType { get; set; }

    public JudgeTypesAttributes(TestType testType)
    {
        TestType = testType;
    }
}