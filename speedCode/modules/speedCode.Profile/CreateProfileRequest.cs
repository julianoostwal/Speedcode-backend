using System.Text.Json.Serialization;
using speedCode.Profile;

namespace speedCode.Profile;

public class CreateProfileRequest
{
    public Guid id { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public string? avatarUrl { get; set; }
    public string? nickName { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Rol role { get; set; }
}