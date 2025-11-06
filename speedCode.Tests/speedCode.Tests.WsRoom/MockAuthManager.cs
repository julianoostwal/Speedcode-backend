using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace speedCode.security;

public static class AuthManager
{
    private static string _jwtSecret = "";

    public static void ConfigureJwt(WebApplicationBuilder builder)
    {
        _jwtSecret = Environment.GetEnvironmentVariable("SupabaseAuth__JwtSecret")
                     ?? builder.Configuration["SupabaseAuth:JwtSecret"]
                     ?? throw new InvalidOperationException(
                         "Missing JWT secret — set SupabaseAuth:JwtSecret in user-secrets or environment.");

        var issuer = builder.Configuration["SupabaseAuth:Authority"]
                     ?? Environment.GetEnvironmentVariable("SupabaseAuth__Authority")
                     ?? "speedCode";

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtOptions =>
            {
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret)),
                    ValidIssuer = issuer,
                    ValidAudience = "authenticated"
                };
            });
    }

    /// <summary>
    /// 测试环境可调用此方法手动设置 JWT Secret。
    /// </summary>
    public static void SetJwtSecret(string secret)
    {
        if (string.IsNullOrWhiteSpace(secret))
            throw new ArgumentException("JWT secret cannot be null or empty", nameof(secret));

        _jwtSecret = secret;
    }

    public static bool ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        if (string.IsNullOrWhiteSpace(_jwtSecret))
            throw new InvalidOperationException(
                "JWT secret not configured — call ConfigureJwt() or SetJwtSecret() first.");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSecret);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            }, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static string GenerateJwtToken(string userId, int expiresMinutes = 60)
    {
        if (string.IsNullOrWhiteSpace(_jwtSecret))
            throw new InvalidOperationException(
                "JWT secret not configured — call ConfigureJwt() or SetJwtSecret() first.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        var token = new JwtSecurityToken(
            audience: "authenticated",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}