using System.Text.Json.Serialization;
using speedCode.Profile;

namespace speedCode.Profile;

public class ProfileRequest
{
    public string email { get; set; }
    public string password { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string? avatarUrl { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Rol role { get; set; }
}