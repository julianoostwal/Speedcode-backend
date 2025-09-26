using System.ComponentModel;

namespace speedCode.codeTesting.enums;

public enum JudgeCodeTestStatus
{
    [Description("not passed")] NotPassed,
    [Description("passed")] Passed,
    [Description("error")] Error
    
}