using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using speedCode.Data;
using speedCode.Model;
using System.Security.Claims;

namespace speedCode.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ProfileController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetMyProfile()
        {
            string? userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                   ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized("User ID not found in token.");

            if (!Guid.TryParse(userIdString, out Guid userId))
                return BadRequest("Invalid user ID format.");

            var profile = _dbContext.Profiles.FirstOrDefault(p => p.id == userId);
            if (profile == null)
                return NotFound("Profile not found.");

            return Ok(profile);
        }
    }
}