using Microsoft.AspNetCore.Mvc;
using speedCode.Data;
using speedCode.Model;
using speedCode.dto;

namespace speedCode.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfilesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProfilesController(AppDbContext context)
    {
        _context = context;
    }

    // GET all
    [HttpGet]
    public IActionResult GetProfiles()
    {
        var profiles = _context.Profiles.ToList();
        return Ok(profiles);
    }

    // GET by id
    [HttpGet("id/{id}")]
    public IActionResult GetProfile(Guid id)
    {
        var profile = _context.Profiles.Find(id);
        if (profile == null)
            return NotFound();

        var roomUser = _context.RoomUser.FirstOrDefault(ru => ru.profileId == profile.id);
        if (roomUser?.roomId == null)
            return Ok("Profile: id: " + profile.id + ", role: " + profile.role);

        var room = _context.Rooms.Find(roomUser.roomId);
        if (room == null)
            return Ok("Profile: id: " + profile.id + ", role: " + profile.role + ", room niet gevonden");

        var exerciseUser = _context.ProfileExercise.FirstOrDefault(pe => pe.profileId == profile.id);
        if (exerciseUser?.exerciseId == null)
            return Ok("Profile: id: " + profile.id + ", role: " + profile.role);

        var exercise = _context.Exercise.Find(exerciseUser.exerciseId);
        if (exercise == null)
            return Ok("Profile: id: " + profile.id + ", role: " + profile.role + ", is in room: " + room.roomName + ", exercise niet gevonden");

        return Ok("Profile: id: " + profile.id + ", role: " + profile.role + ", is in room: " + room.roomName + ", Exercise: " + exercise.title);
    }

    // POST
    /*[HttpPost] moet verder aan gewerkt worden (3-10-2025)
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        using var httpClient = new HttpClient();
        var supabaseUrl = "https://supabase.speedcode.tech";
        var serviceRoleKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJzdXBhYmFzZSIsImlhdCI6MTc1NzU5MzgwMCwiZXhwIjo0OTEzMjY3NDAwLCJyb2xlIjoic2VydmljZV9yb2xlIn0.U5ovw3nz1VKJJCbKfzARWXuHTUr73U1LNtyBNyhT6pg";
        httpClient.BaseAddress = new Uri(supabaseUrl);
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", serviceRoleKey);
        httpClient.DefaultRequestHeaders.Add("apikey", serviceRoleKey);

        var payload = new
        {
            email = request.email,
            password = request.password,
            email_confirm = true
        };
        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("/auth/v1/admin/users", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return BadRequest($"Failed to create user in Supabase Auth: {error}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var doc = System.Text.Json.JsonDocument.Parse(responseBody);
        
        if (!doc.RootElement.TryGetProperty("id", out var idElement))
        {
            return BadRequest("Supabase did not return a user id. Response: " + responseBody);
        }
        
        var userId = idElement.GetString();

        if (!Guid.TryParse(userId, out Guid supabaseUserId))
            return BadRequest("Invalid user ID returned from Supabase.");

        if (_context.Profiles.Any(p => p.id == supabaseUserId))
            return Conflict("Profile already exists for this user.");

        var profile = _context.Profiles.FirstOrDefault(p => p.id == supabaseUserId);
        if (profile == null)
            return StatusCode(500, "Profile was not created in the database.");

        return CreatedAtAction(nameof(GetProfile), new { id = profile.id }, profile);
    }

    // UPDATE
    [HttpPut("{id}")]
    public IActionResult UpdateProfile(Guid id, [FromBody] Profiles updatedProfile)
    {
        if (id != updatedProfile.id)
            return BadRequest();

        var profile = _context.Profiles.Find(id);
        if (profile == null)
            return NotFound();

        profile.avatarUrl = updatedProfile.avatarUrl;
        profile.nickName = updatedProfile.nickName;
        profile.role = updatedProfile.role;

        _context.SaveChanges();
        return NoContent();
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProfile(Guid id)
    {
        var profile = _context.Profiles.Find(id);

        var supabaseUrl = "https://supabase.speedcode.tech";
        var serviceRoleKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJzdXBhYmFzZSIsImlhdCI6MTc1NzU5MzgwMCwiZXhwIjo0OTEzMjY3NDAwLCJyb2xlIjoic2VydmljZV9yb2xlIn0.U5ovw3nz1VKJJCbKfzARWXuHTUr73U1LNtyBNyhT6pg";
        using (var httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(supabaseUrl);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", serviceRoleKey);
            httpClient.DefaultRequestHeaders.Add("apikey", serviceRoleKey);

            var response = await httpClient.DeleteAsync($"/auth/v1/admin/users/{id}");
        }

        var achievements = _context.ProfileAchievement.Where(pa => pa.profileId == id);
        if (achievements.Any())
            _context.ProfileAchievement.RemoveRange(achievements);

        var exercises = _context.ProfileExercise.Where(pe => pe.profileId == id);
        if (exercises.Any())
            _context.ProfileExercise.RemoveRange(exercises);

        var roomUsers = _context.RoomUser.Where(ru => ru.profileId == id);
        if (roomUsers.Any())
            _context.RoomUser.RemoveRange(roomUsers);

        if (profile != null)
            _context.Profiles.Remove(profile);

        if (profile == null && !achievements.Any() && !exercises.Any() && !roomUsers.Any())
            return NotFound();

        _context.SaveChanges();
        return NoContent();
    }*/
}