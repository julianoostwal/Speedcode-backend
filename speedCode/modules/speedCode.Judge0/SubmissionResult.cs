using System.Text.Json.Serialization;
using speedCode.Core.judge0.dto;
using speedCode.Judge0;

namespace speedCode.dto;

public record SubmissionResult(
    Judge0Response Result,
    string Stdin
);