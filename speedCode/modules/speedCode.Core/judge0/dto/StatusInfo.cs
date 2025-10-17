using System.Text.Json.Serialization;

namespace speedCode.Core.judge0.dto;


public record StatusInfo(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("description")]
    string Description
);