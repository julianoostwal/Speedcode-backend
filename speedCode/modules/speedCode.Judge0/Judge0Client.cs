using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using speedCode.Core.judge0.dto;
using speedCode.dto;

namespace speedCode.Judge0;

public class Judge0Client
{
    private readonly HttpClient _httpClient;

    public Judge0Client(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<SubmissionResult> SubmitCodeAsync(SubmissionRequest submissionRequest)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(submissionRequest, jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(
            "/submissions?base64_encoded=false&wait=true", content);
        var body = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        Judge0Response responseObj = await response.Content.ReadFromJsonAsync<Judge0Response>();
        return new SubmissionResult(responseObj, submissionRequest.Stdin);
    }
}