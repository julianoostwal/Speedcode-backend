using speedCode.codeTesting.enums;

namespace speedCode.codeTesting;

public class JudgeCodeTestResults
{
    public List<JudgeCodeTestLog> Logs { get; set; } = new List<JudgeCodeTestLog>();
    public JudgeCodeTestStatus Status { get; set; } = JudgeCodeTestStatus.Unknow;
}