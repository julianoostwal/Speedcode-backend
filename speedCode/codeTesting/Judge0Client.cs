using System.Text;
using System.Text.Json;
using speedCode.dto;

namespace speedCode.codeTesting;

public class Judge0Client
{
    private readonly HttpClient _httpClient;

    public Judge0Client(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<SubmissionResult> SubmitCodeAsync(string sourceCode, int languageId)
    {
        sourceCode = convertSourceCode(sourceCode);
        var payload = new SubmissionRequest(sourceCode, languageId);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(payload, jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(
            "/submissions?base64_encoded=false&wait=true", content);
        var body = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SubmissionResult>();
    }

    public string convertSourceCode(string sourceCode)
    {
        sourceCode = sourceCode.Trim();
        if (sourceCode.StartsWith("'") && sourceCode.EndsWith("'"))
        {
            sourceCode = sourceCode[1..^1];
        }

        sourceCode = sourceCode.Replace("\"", "\\\"");
        return sourceCode;
    }
}