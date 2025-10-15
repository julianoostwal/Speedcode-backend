using speedCode.dto;
using speedCode.Model;

namespace speedCode.codeTesting.attributes;

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