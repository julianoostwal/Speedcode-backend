using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace speedCode.security;

public static class AuthManager
{
    public static void ConfigureJwt(WebApplicationBuilder builder)
    {
        var supabaseAuth = builder.Configuration.GetSection("SupabaseAuth").Get<SupabaseAuth>();
        var byteKey = Encoding.UTF8.GetBytes(supabaseAuth.jwtSecret);
        var issuer = supabaseAuth.Authority;
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtOptions =>
            {
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(byteKey),
                    ValidIssuer = issuer,
                    ValidAudience = "authenticated",
                };
            });
    }
}