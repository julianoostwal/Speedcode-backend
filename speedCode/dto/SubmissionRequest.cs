using System.Text.Json.Serialization;

namespace speedCode.dto;

public record SubmissionRequest(
    [property: JsonPropertyName("source_code")] string SourceCode,
    [property: JsonPropertyName("language_id")] int LanguageId
);