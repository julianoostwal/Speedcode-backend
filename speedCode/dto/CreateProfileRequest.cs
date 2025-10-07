using System.Text.Json.Serialization;
using speedCode.Model;

public class CreateProfileRequest
{
    public string email { get; set; }
    public string password { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string? avatarUrl { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Profiles.Rol role { get; set; }
}