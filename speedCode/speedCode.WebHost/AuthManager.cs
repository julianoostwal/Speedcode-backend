using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace speedCode.security;

public static class AuthManager
{
    private static string _jwtSecret = "";

    public static void ConfigureJwt(WebApplicationBuilder builder)
    {
        var supabaseAuth = builder.Configuration.GetSection("SupabaseAuth").Get<SupabaseAuth>();
        _jwtSecret = supabaseAuth.jwtSecret;
        var issuer = supabaseAuth.Authority;

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
                    ValidAudience = "authenticated",
                };
            });
    }

    public static bool ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSecret);
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
        };
        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static string? GetClientIdFromToken(string token)
    {
        if (!ValidateToken(token)) return null;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        }
        catch
        {
            return null;
        }
    }

    public static string GenerateJwtToken(string userId, int expiresMinutes = 60)
    {
        if (string.IsNullOrWhiteSpace(_jwtSecret))
            throw new InvalidOperationException("JWT secret is not configured , using ConfigureJwt() first!");

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