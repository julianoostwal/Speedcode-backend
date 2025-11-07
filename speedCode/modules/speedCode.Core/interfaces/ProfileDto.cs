namespace speedCode.Core.interfaces;

public class ProfileDto
{
    public Guid Id { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
    public string NickName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

