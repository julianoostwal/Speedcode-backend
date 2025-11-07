namespace speedCode.Core.interfaces;

public class ProfileDto
{
    public Guid id { get; set; }
    public string avatarUrl { get; set; } = string.Empty;
    public string nickName { get; set; } = string.Empty;
    public string role { get; set; } = string.Empty;
    public DateTime createdAt { get; set; }
}


