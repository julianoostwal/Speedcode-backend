using System.Text.Json.Serialization;

namespace speedCode.Core.judge0.dto;

public class SubmissionRequest
{
    [JsonPropertyName("source_code")] public string SourceCode { get; set; }

    [JsonPropertyName("language_id")] public int LanguageId { get; set; }

    [JsonPropertyName("stdin")] public string? Stdin { get; set; }

    public SubmissionRequest()
    {
    }

    public SubmissionRequest(string sourceCode, int languageId, string? stdin = null)
    {
        SourceCode = sourceCode;
        LanguageId = languageId;
        Stdin = stdin;
    }
}