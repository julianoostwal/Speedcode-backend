using System.ComponentModel;
namespace speedCode.Core.judge0.enums;


public enum JudgeCodeTestStatus
{
    [Description("not passed")] NotPassed,
    [Description("passed")] Passed,
    [Description("error")] Error,
    [Description("unknow")] Unknow
}