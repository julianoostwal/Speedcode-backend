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

        var profileAchievement = _context.ProfileAchievement.Where(pa => pa.profileId == profile.id).ToList();
        if (profileAchievement == null || profileAchievement.Count == 0)
            return Ok("Profile: id: " + profile.id + ", role: " + profile.role + ", is in room: " + room.roomName + ", Exercise: " + exercise.title + ", heeft nog geen achievements");
        
        var achievements = new List<string>();
        foreach (var pa in profileAchievement)
        {
            var achievement = _context.Achievement.Find(pa.achievementId);
            if (achievement != null)
                achievements.Add(achievement.achievementName);
        }
        if (achievements.Count == 0)
            return Ok("Profile: id: " + profile.id + ", role: " + profile.role + ", is in room: " + room.roomName + ", Exercise: " + exercise.title + ", heeft nog geen achievements");

        return Ok("Profile: id: " + profile.id + ", role: " + profile.role + ", is in room: " + room.roomName + ", Exercise: " + exercise.title + ", Achievements: " + string.Join(", ", achievements));
    }

    // GET by email
    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetProfileByEmail(string email)
    {
        var supabaseUrl = "https://supabase.speedcode.tech";
        var serviceRoleKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJzdXBhYmFzZSIsImlhdCI6MTc1NzU5MzgwMCwiZXhwIjo0OTEzMjY3NDAwLCJyb2xlIjoic2VydmljZV9yb2xlIn0.U5ovw3nz1VKJJCbKfzARWXuHTUr73U1LNtyBNyhT6pg";
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(supabaseUrl);
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", serviceRoleKey);
        httpClient.DefaultRequestHeaders.Add("apikey", serviceRoleKey);

        var requestUrl = $"/auth/v1/admin/users?email={email}";
        var response = await httpClient.GetAsync(requestUrl);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return BadRequest($"Failed to fetch user from Supabase Auth: {responseBody}");

        var doc = System.Text.Json.JsonDocument.Parse(responseBody);
        if (!doc.RootElement.TryGetProperty("users", out var usersArray) || usersArray.GetArrayLength() == 0)
            return NotFound("No user found with this email in Supabase Auth.");

        // Filter users by exact email match
        var userElement = usersArray.EnumerateArray()
            .FirstOrDefault(u => u.TryGetProperty("email", out var e) && e.GetString() == email);

        if (userElement.ValueKind == System.Text.Json.JsonValueKind.Undefined)
            return NotFound("No user found with this email in Supabase Auth.");

        var userId = userElement.GetProperty("id").GetString();
        var userEmail = userElement.GetProperty("email").GetString();

        if (!Guid.TryParse(userId, out Guid supabaseUserId))
            return BadRequest("Invalid user ID returned from Supabase.");

        var profile = _context.Profiles.FirstOrDefault(p => p.id == supabaseUserId);
        if (profile == null)
            return NotFound("Profile not found in database for this Supabase user.");

        var roomUser = _context.RoomUser.FirstOrDefault(ru => ru.profileId == profile.id);
        if (roomUser?.roomId == null)
            return Ok("Profile: id: " + profile.id + ", role: " + profile.role + ", email: " + userEmail);

        var room = _context.Rooms.Find(roomUser.roomId);
        if (room == null)
            return Ok("Profile: id: " + profile.id + ", role: " + profile.role + ", email: " + userEmail + ", room niet gevonden");

        var exerciseUser = _context.ProfileExercise.FirstOrDefault(pe => pe.profileId == profile.id);
        if (exerciseUser?.exerciseId == null)
            return Ok("Profile: id: " + profile.id + ", role: " + profile.role + ", email: " + userEmail);

        var exercise = _context.Exercise.Find(exerciseUser.exerciseId);
        if (exercise == null)
            return Ok("Profile: id: " + profile.id + ", role: " + profile.role + ", email: " + userEmail + ", is in room: " + room.roomName + ", exercise niet gevonden");
        
        var profileAchievement = _context.ProfileAchievement.Where(pa => pa.profileId == profile.id).ToList();
        if (profileAchievement == null || profileAchievement.Count == 0)
            return Ok("Profile: id: " + profile.id + ", role: " + profile.role + ", email: " + userEmail + ", is in room: " + room.roomName + ", Exercise: " + exercise.title + ", heeft nog geen achievements");
        
        var achievements = new List<string>();
        foreach (var pa in profileAchievement)
        {
            var achievement = _context.Achievement.Find(pa.achievementId);
            if (achievement != null)
                achievements.Add(achievement.achievementName);
        }
        if (achievements.Count == 0)
            return Ok("Profile: id: " + profile.id + ", role: " + profile.role + ", email: " + userEmail + ", is in room: " + room.roomName + ", Exercise: " + exercise.title + ", heeft nog geen achievements");

        return Ok("Profile: id: " + profile.id + ", role: " + profile.role + ", email: " + userEmail + ", is in room: " + room.roomName + ", Exercise: " + exercise.title + ", Achievements: " + string.Join(", ", achievements));
    }

    // POST
    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if user already exists in Supabase Auth
        using var httpClient = new HttpClient();
        var supabaseUrl = "https://supabase.speedcode.tech";
        var serviceRoleKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJzdXBhYmFzZSIsImlhdCI6MTc1NzU5MzgwMCwiZXhwIjo0OTEzMjY3NDAwLCJyb2xlIjoic2VydmljZV9yb2xlIn0.U5ovw3nz1VKJJCbKfzARWXuHTUr73U1LNtyBNyhT6pg";
        httpClient.BaseAddress = new Uri(supabaseUrl);
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", serviceRoleKey);
        httpClient.DefaultRequestHeaders.Add("apikey", serviceRoleKey);

        var checkResponse = await httpClient.GetAsync($"/auth/v1/admin/users?email={request.email}");
        var checkBody = await checkResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"Supabase Auth response for {request.email}: {checkBody}");
        var checkDoc = System.Text.Json.JsonDocument.Parse(checkBody);

        if (checkDoc.RootElement.TryGetProperty("users", out var usersArray))
        {
            // Only return conflict if any user has the exact email
            bool emailExists = usersArray.EnumerateArray()
                .Any(u => u.TryGetProperty("email", out var e) && e.GetString() == request.email);

            if (emailExists)
                return Conflict("A user with this email already exists.");
        }

        // Check if nickname already exists in local DB
        var nickName = $"{request.firstName} {request.lastName}";
        if (_context.Profiles.Any(p => p.nickName == nickName))
            return Conflict("A profile with this nickname already exists.");

        var payload = new
        {
            email = request.email,
            password = request.password,
            email_confirm = true,
            avatar_url = request.avatarUrl,
            role = request.role.ToString(),
            user_metadata = new
            {
                first_name = request.firstName,
                last_name = request.lastName
            }
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

        var profile = _context.Profiles.FirstOrDefault(p => p.id == supabaseUserId);
        if (profile == null)
            return StatusCode(500, "Profile was not created in the database.");

        profile.avatarUrl = request.avatarUrl;
        profile.role = request.role;
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetProfile), new { id = profile.id }, profile);
    }

    // UPDATE
    /*[HttpPut("{id}")]
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
    /*[HttpDelete("{id}")]
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