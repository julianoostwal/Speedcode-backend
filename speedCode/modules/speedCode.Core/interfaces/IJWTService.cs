namespace speedCode.Core.interfaces
{
    public interface IJwtService
    {
        string GenerateToken(string userId, int expiresMinutes = 60);
        bool ValidateToken(string token);
        string? GetClientIdFromToken(string token);
    }
}