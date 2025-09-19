namespace speedCode.security;

public class SupabaseAuth
{
    public string Authority { get; set; } = string.Empty;
    public string MetadataAddress { get; set; } = string.Empty;
    public string jwtSecret { get; set; } = string.Empty;
}