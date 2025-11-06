using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace speedCode.Infrastructure
{
    public class WebHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WebHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetClientIdByTokenInHeader()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
                throw new Exception("HttpContext not found");

            if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
                throw new Exception("Authorization header not found");

            var token = authHeader.ToString().Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("Token not found");

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
                throw new Exception("Invalid token");

            var jwtToken = handler.ReadJwtToken(token);

            var clientId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (clientId == null)
            {
                throw new Exception("Invalid token");
            }

            return Guid.Parse(clientId);
        }

        public string? GetClientId()
        {
            throw new NotImplementedException();
        }
    }
}