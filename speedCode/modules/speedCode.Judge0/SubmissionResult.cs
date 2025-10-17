using System.Text.Json.Serialization;
using speedCode.Core.judge0.dto;

namespace speedCode.dto;

public record SubmissionResult(
    [property: JsonPropertyName("stdout")] string Stdout,
    [property: JsonPropertyName("stderr")] string Stderr,
    [property: JsonPropertyName("compile_output")] string CompileOutput,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("token")] string Token,
    [property: JsonPropertyName("time")] string Time,
    [property: JsonPropertyName("memory")] int Memory,
    [property: JsonPropertyName("status")] StatusInfo Status
);