using System.Text.Json.Serialization;
using speedCode.Core.judge0.dto;
using speedCode.Judge0;

namespace speedCode.Judge0;

public record SubmissionResult(
    Judge0Response Result,
    string Stdin
);