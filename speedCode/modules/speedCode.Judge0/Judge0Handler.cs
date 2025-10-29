using System.Text.RegularExpressions;
using speedCode.Core.judge0.dto;
namespace speedCode.Judge0;

public class Judge0Handler
{
    public readonly Judge0Client _judge0Client;

    public Judge0Handler(Judge0Client judge0Client)
    {
        _judge0Client = judge0Client;
    }

    public async Task<SubmissionResult> HandleSubmitCodeAsync(SubmissionRequest submissionRequest, string? ACRegex)
    {
        if (!string.IsNullOrEmpty(ACRegex) && Regex.IsMatch(submissionRequest.SourceCode, ACRegex))
        {
            return new SubmissionResult(new Judge0Response(
                "", "", "", "", "", "", 0,
                new StatusInfo(-1, "Cheating detected!")
            ), "");
        }

        return await _judge0Client.SubmitCodeAsync(submissionRequest);
    }
}