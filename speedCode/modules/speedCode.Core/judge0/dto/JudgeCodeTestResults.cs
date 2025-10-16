using speedCode.Core.judge0.enums;

namespace speedCode.Core.judge0.dto;


public class JudgeCodeTestResults
{
    public List<JudgeCodeTestLog> Logs { get; set; } = new List<JudgeCodeTestLog>();
    public JudgeCodeTestStatus Status { get; set; } = JudgeCodeTestStatus.Unknow;
}