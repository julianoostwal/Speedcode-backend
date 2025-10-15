using System.Text.Json.Serialization;

namespace speedCode.dto;

public record StatusInfo(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("description")]
    string Description
);