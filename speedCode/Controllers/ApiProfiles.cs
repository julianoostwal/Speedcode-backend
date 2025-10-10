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

    // GET all profiles
    [HttpGet]
    public async Task<IActionResult> GetFullProfiles()
    {
        var supabaseUrl = "https://supabase.speedcode.tech";
        var serviceRoleKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJzdXBhYmFzZSIsImlhdCI6MTc1NzU5MzgwMCwiZXhwIjo0OTEzMjY3NDAwLCJyb2xlIjoic2VydmljZV9yb2xlIn0.U5ovw3nz1VKJJCbKfzARWXuHTUr73U1LNtyBNyhT6pg";
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(supabaseUrl);
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", serviceRoleKey);
        httpClient.DefaultRequestHeaders.Add("apikey", serviceRoleKey);

        var response = await httpClient.GetAsync("/auth/v1/admin/users");
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return BadRequest($"Failed to fetch users from Supabase Auth: {responseBody}");

        var doc = System.Text.Json.JsonDocument.Parse(responseBody);
        if (!doc.RootElement.TryGetProperty("users", out var usersArray))
            return Ok(new List<object>());

        var profiles = _context.Profiles.ToList();
        var roomUsers = _context.RoomUser.ToList();
        var rooms = _context.Rooms.ToList();
        var profileExercises = _context.ProfileExercise.ToList();
        var exercises = _context.Exercise.ToList();
        var exerciseTests = _context.ExerciseTest.ToList();
        var profileAchievements = _context.ProfileAchievement.ToList();
        var achievements = _context.Achievement.ToList();
        var testCodes = _context.TestCode.ToList();

        var result = new List<object>();

        foreach (var userElement in usersArray.EnumerateArray())
        {
            var userIdStr = userElement.GetProperty("id").GetString();
            var email = userElement.GetProperty("email").GetString();

            Guid userId;
            if (!Guid.TryParse(userIdStr, out userId))
                continue;

            var profile = profiles.FirstOrDefault(p => p.id == userId);

            var userRoomUsers = roomUsers.Where(ru => ru.profileId == userId).Select(ru => new
            {
                roomUserId = ru.id,
                room = rooms.FirstOrDefault(r => r.id == ru.roomId)
            }).ToList();

            var userProfileExercises = profileExercises.Where(pe => pe.profileId == userId).Select(pe => new
            {
                profileExerciseId = pe.id,
                exercise = exercises.FirstOrDefault(ex => ex.id == pe.exerciseId)
            }).ToList();

            var userProfileAchievements = profileAchievements.Where(pa => pa.profileId == userId).Select(pa => new
            {
                profileAchievementId = pa.id,
                achievement = achievements.FirstOrDefault(a => a.id == pa.achievementId)
            }).ToList();

            var userProfileTestcodes = testCodes.Where(pa => pa.authorId == userId).ToList();

            var ExerciseTests = exerciseTests.Where(et => et.testId == userProfileTestcodes.FirstOrDefault()?.id).ToList();

            result.Add(new
            {
                id = userIdStr,
                email,
                avatarUrl = profile?.avatarUrl,
                createdAt = profile?.createdAt,
                nickName = profile?.nickName,
                role = profile?.role.ToString() ?? "user",
                rooms = userRoomUsers.Select(ru => new
                {
                    roomUserId = ru.roomUserId,
                    roomId = ru.room?.id,
                    roomName = ru.room?.roomName,
                    roomLevel = ru.room?.roomLevel.ToString(),
                    isPrivate = ru.room?.isPrivate,
                    createdAt = ru.room?.createdAt
                }),
                exercises = userProfileExercises.Select(pe => new
                {
                    profileExerciseId = pe.profileExerciseId,
                    exerciseId = pe.exercise?.id,
                    title = pe.exercise?.title,
                    description = pe.exercise?.description,
                    points = pe.exercise?.points,
                    createdAt = pe.exercise?.createdAt
                }),
                testcodes = userProfileTestcodes.Select(pt => new
                {
                    id = pt.id,
                    authorId = pt.authorId,
                    expectedResult = pt.expectedResult,
                    type = pt.Type.ToString(),
                    createdAt = pt.createdAt
                }),
                exerciseTests = ExerciseTests.Select(et => new
                {
                    id = et.id,
                    testId = et.testId,
                    exerciseId = et.exerciseId
                }),
                achievements = userProfileAchievements.Select(pa => new
                {
                    profileAchievementId = pa.profileAchievementId,
                    achievementId = pa.achievement?.id,
                    achievementName = pa.achievement?.achievementName,
                    achievementDescription = pa.achievement?.achievementDescription,
                    achievementIcon = pa.achievement?.achievementIcon
                })
            });
        }

        return Ok(result);
    }

    // GET by id
    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetProfile(Guid id)
    {
        var profile = _context.Profiles.Find(id);
        if (profile == null)
            return NotFound();

        var supabaseUrl = "https://supabase.speedcode.tech";
        var serviceRoleKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJzdXBhYmFzZSIsImlhdCI6MTc1NzU5MzgwMCwiZXhwIjo0OTEzMjY3NDAwLCJyb2xlIjoic2VydmljZV9yb2xlIn0.U5ovw3nz1VKJJCbKfzARWXuHTUr73U1LNtyBNyhT6pg";
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(supabaseUrl);
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", serviceRoleKey);
        httpClient.DefaultRequestHeaders.Add("apikey", serviceRoleKey);
    
        var response = await httpClient.GetAsync($"/auth/v1/admin/users/{id}");
        var responseBody = await response.Content.ReadAsStringAsync();

        string userEmail = null;

        if (response.IsSuccessStatusCode)
        {
            var doc = System.Text.Json.JsonDocument.Parse(responseBody);
            if (doc.RootElement.TryGetProperty("email", out var emailElement))
                userEmail = emailElement.GetString();
        }

        var email = userEmail;

        var roomUsers = _context.RoomUser.Where(ru => ru.profileId == id).ToList();
        var rooms = _context.Rooms.ToList();
        var profileExercises = _context.ProfileExercise.Where(pe => pe.profileId == id).ToList();
        var exercises = _context.Exercise.ToList();
        var profileAchievements = _context.ProfileAchievement.Where(pa => pa.profileId == id).ToList();
        var achievements = _context.Achievement.ToList();
        var testCodes = _context.TestCode.Where(pt => pt.authorId == id).ToList();
        var exerciseTests = _context.ExerciseTest.Where(et => et.testId == testCodes.FirstOrDefault().id).ToList();

        var result = new
        {
            id = id.ToString(),
            email,
            avatarUrl = profile.avatarUrl,
            createdAt = profile.createdAt,
            nickName = profile.nickName,
            role = profile.role.ToString(),
            rooms = roomUsers.Select(ru => new
            {
                roomUserId = ru.id,
                roomId = ru.roomId,
                roomName = rooms.FirstOrDefault(r => r.id == ru.roomId)?.roomName,
                roomLevel = rooms.FirstOrDefault(r => r.id == ru.roomId)?.roomLevel.ToString(),
                isPrivate = rooms.FirstOrDefault(r => r.id == ru.roomId)?.isPrivate,
                createdAt = rooms.FirstOrDefault(r => r.id == ru.roomId)?.createdAt
            }),
            exercises = profileExercises.Select(pe => new
            {
                profileExerciseId = pe.id,
                exerciseId = pe.exerciseId,
                title = exercises.FirstOrDefault(ex => ex.id == pe.exerciseId)?.title,
                description = exercises.FirstOrDefault(ex => ex.id == pe.exerciseId)?.description,
                points = exercises.FirstOrDefault(ex => ex.id == pe.exerciseId)?.points,
                createdAt = exercises.FirstOrDefault(ex => ex.id == pe.exerciseId)?.createdAt
            }),
            exerciseTests = exerciseTests.Select(et => new
            {
                id = et.id,
                testId = et.testId,
                exerciseId = et.exerciseId
            }),
            testcodes = testCodes.Select(pt => new
            {
                id = pt.id,
                authorId = pt.authorId,
                expectedResult = pt.expectedResult,
                type = pt.Type.ToString(),
                createdAt = pt.createdAt
            }),
            achievements = profileAchievements.Select(pa => new
            {
                profileAchievementId = pa.id,
                achievementId = pa.achievementId,
                achievementName = achievements.FirstOrDefault(a => a.id == pa.achievementId)?.achievementName,
                achievementDescription = achievements.FirstOrDefault(a => a.id == pa.achievementId)?.achievementDescription,
                achievementIcon = achievements.FirstOrDefault(a => a.id == pa.achievementId)?.achievementIcon
            })
        };

        return Ok(result);
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

        var response = await httpClient.GetAsync($"/auth/v1/admin/users?email={email}");
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return BadRequest($"Failed to fetch user from Supabase Auth: {responseBody}");

        var doc = System.Text.Json.JsonDocument.Parse(responseBody);
        if (!doc.RootElement.TryGetProperty("users", out var usersArray) || usersArray.GetArrayLength() == 0)
            return NotFound("No user found with this email in Supabase Auth.");

        var userElement = usersArray.EnumerateArray()
            .FirstOrDefault(u => u.TryGetProperty("email", out var e) && e.GetString() == email);

        if (userElement.ValueKind == System.Text.Json.JsonValueKind.Undefined)
            return NotFound("No user found with this email in Supabase Auth.");

        var userId = userElement.GetProperty("id").GetString();
        var userEmail = userElement.GetProperty("email").GetString();

        if (!Guid.TryParse(userId, out Guid supabaseUserId))
            return BadRequest("Invalid user ID returned from Supabase.");

        var profile = _context.Profiles.Find(supabaseUserId);
        if (profile == null)
            return NotFound();

        var roomUsers = _context.RoomUser.Where(ru => ru.profileId == supabaseUserId).ToList();
        var rooms = _context.Rooms.ToList();
        var profileExercises = _context.ProfileExercise.Where(pe => pe.profileId == supabaseUserId).ToList();
        var exercises = _context.Exercise.ToList();
        var profileAchievements = _context.ProfileAchievement.Where(pa => pa.profileId == supabaseUserId).ToList();
        var achievements = _context.Achievement.ToList();
        var testCodes = _context.TestCode.Where(pt => pt.authorId == supabaseUserId).ToList();
        var exerciseTests = _context.ExerciseTest.Where(et => et.testId == testCodes.FirstOrDefault().id).ToList();

        var result = new
        {
            id = profile.id.ToString(),
            email,
            avatarUrl = profile.avatarUrl,
            createdAt = profile.createdAt,
            nickName = profile.nickName,
            role = profile.role.ToString(),
            rooms = roomUsers.Select(ru => new
            {
                roomUserId = ru.id,
                roomId = ru.roomId,
                roomName = rooms.FirstOrDefault(r => r.id == ru.roomId)?.roomName,
                roomLevel = rooms.FirstOrDefault(r => r.id == ru.roomId)?.roomLevel.ToString(),
                isPrivate = rooms.FirstOrDefault(r => r.id == ru.roomId)?.isPrivate,
                createdAt = rooms.FirstOrDefault(r => r.id == ru.roomId)?.createdAt
            }),
            exercises = profileExercises.Select(pe => new
            {
                profileExerciseId = pe.id,
                exerciseId = pe.exerciseId,
                title = exercises.FirstOrDefault(ex => ex.id == pe.exerciseId)?.title,
                description = exercises.FirstOrDefault(ex => ex.id == pe.exerciseId)?.description,
                points = exercises.FirstOrDefault(ex => ex.id == pe.exerciseId)?.points,
                createdAt = exercises.FirstOrDefault(ex => ex.id == pe.exerciseId)?.createdAt
            }),
            exerciseTests = exerciseTests.Select(et => new
            {
                id = et.id,
                testId = et.testId,
                exerciseId = et.exerciseId
            }),
            testcodes = testCodes.Select(pt => new
            {
                id = pt.id,
                authorId = pt.authorId,
                expectedResult = pt.expectedResult,
                type = pt.Type.ToString(),
                createdAt = pt.createdAt
            }),
            achievements = profileAchievements.Select(pa => new
            {
                profileAchievementId = pa.id,
                achievementId = pa.achievementId,
                achievementName = achievements.FirstOrDefault(a => a.id == pa.achievementId)?.achievementName,
                achievementDescription = achievements.FirstOrDefault(a => a.id == pa.achievementId)?.achievementDescription,
                achievementIcon = achievements.FirstOrDefault(a => a.id == pa.achievementId)?.achievementIcon
            })
        };

        return Ok(result);
    }

    // POST
    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] ProfileRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

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
            bool emailExists = usersArray.EnumerateArray()
                .Any(u => u.TryGetProperty("email", out var e) && e.GetString() == request.email);

            if (emailExists)
                return Conflict("A user with this email already exists.");
        }

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

    // Update user by id
    [HttpPut("id/{id}")]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateProfileRequest updatedProfile)
    {
        var profile = _context.Profiles.Find(id);
        if (profile == null)
            return NotFound();

        string newNickName = $"{updatedProfile.firstName} {updatedProfile.lastName}";
        if (!string.IsNullOrWhiteSpace(updatedProfile.firstName) && !string.IsNullOrWhiteSpace(updatedProfile.lastName) && profile.nickName != newNickName)
        {
            if (_context.Profiles.Any(p => p.id != id && p.nickName == newNickName))
                return Conflict("A profile with this nickname already exists.");
            profile.nickName = newNickName;
        }

        if (!string.IsNullOrWhiteSpace(updatedProfile.avatarUrl))
        {
            if (profile.avatarUrl != updatedProfile.avatarUrl)
                profile.avatarUrl = updatedProfile.avatarUrl;
        }

        if (updatedProfile.role != profile.role)
            profile.role = updatedProfile.role;

        _context.SaveChanges();

        var supabaseUrl = "https://supabase.speedcode.tech";
        var serviceRoleKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJzdXBhYmFzZSIsImlhdCI6MTc1NzU5MzgwMCwiZXhwIjo0OTEzMjY3NDAwLCJyb2xlIjoic2VydmljZV9yb2xlIn0.U5ovw3nz1VKJJCbKfzARWXuHTUr73U1LNtyBNyhT6pg";
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(supabaseUrl);
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", serviceRoleKey);
        httpClient.DefaultRequestHeaders.Add("apikey", serviceRoleKey);

        var payloadDict = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(updatedProfile.email) && profile.avatarUrl != updatedProfile.email)
            payloadDict["email"] = updatedProfile.email;

        if (!string.IsNullOrWhiteSpace(updatedProfile.password))
            payloadDict["password"] = updatedProfile.password;

        if (!string.IsNullOrWhiteSpace(updatedProfile.avatarUrl) && profile.avatarUrl != updatedProfile.avatarUrl)
            payloadDict["avatar_url"] = updatedProfile.avatarUrl;

        var userMetadata = new Dictionary<string, object>();
        if (!string.IsNullOrWhiteSpace(updatedProfile.firstName))
            userMetadata["first_name"] = updatedProfile.firstName;
        if (!string.IsNullOrWhiteSpace(updatedProfile.lastName))
            userMetadata["last_name"] = updatedProfile.lastName;

        if (userMetadata.Count > 0)
            payloadDict["user_metadata"] = userMetadata;

        var json = System.Text.Json.JsonSerializer.Serialize(payloadDict);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"/auth/v1/admin/users/{id}", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return BadRequest($"Failed to update user in Supabase Auth: {error}");
        }

        return NoContent();
    }

    // Update user by email
    [HttpPut("email/{email}")]
    public async Task<IActionResult> UpdateProfileByEmail(string email, [FromBody] UpdateProfileRequest updatedProfile)
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

        var userElement = usersArray.EnumerateArray()
            .FirstOrDefault(u => u.TryGetProperty("email", out var e) && e.GetString() == email);

        if (userElement.ValueKind == System.Text.Json.JsonValueKind.Undefined)
            return NotFound("No user found with this email in Supabase Auth.");

        var userId = userElement.GetProperty("id").GetString();
        if (!Guid.TryParse(userId, out Guid supabaseUserId))
            return BadRequest("Invalid user ID returned from Supabase.");

        var profile = _context.Profiles.FirstOrDefault(p => p.id == supabaseUserId);
        if (profile == null)
            return NotFound("Profile not found in database for this Supabase user.");

        string newNickName = $"{updatedProfile.firstName} {updatedProfile.lastName}";
        if (!string.IsNullOrWhiteSpace(updatedProfile.firstName) && !string.IsNullOrWhiteSpace(updatedProfile.lastName) && profile.nickName != newNickName)
        {
            if (_context.Profiles.Any(p => p.id != supabaseUserId && p.nickName == newNickName))
                return Conflict("A profile with this nickname already exists.");
            profile.nickName = newNickName;
        }

        if (!string.IsNullOrWhiteSpace(updatedProfile.avatarUrl) && profile.avatarUrl != updatedProfile.avatarUrl)
            profile.avatarUrl = updatedProfile.avatarUrl;

        if (updatedProfile.role != profile.role)
            profile.role = updatedProfile.role;

        _context.SaveChanges();

        var payloadDict = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(updatedProfile.email) && email != updatedProfile.email)
            payloadDict["email"] = updatedProfile.email;

        if (!string.IsNullOrWhiteSpace(updatedProfile.password))
            payloadDict["password"] = updatedProfile.password;

        if (!string.IsNullOrWhiteSpace(updatedProfile.avatarUrl) && profile.avatarUrl != updatedProfile.avatarUrl)
            payloadDict["avatar_url"] = updatedProfile.avatarUrl;

        var userMetadata = new Dictionary<string, object>();
        if (!string.IsNullOrWhiteSpace(updatedProfile.firstName))
            userMetadata["first_name"] = updatedProfile.firstName;
        if (!string.IsNullOrWhiteSpace(updatedProfile.lastName))
            userMetadata["last_name"] = updatedProfile.lastName;

        if (userMetadata.Count > 0)
            payloadDict["user_metadata"] = userMetadata;

        var json = System.Text.Json.JsonSerializer.Serialize(payloadDict);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var updateResponse = await httpClient.PutAsync($"/auth/v1/admin/users/{userId}", content);

        if (!updateResponse.IsSuccessStatusCode)
        {
            var error = await updateResponse.Content.ReadAsStringAsync();
            return BadRequest($"Failed to update user in Supabase Auth: {error}");
        }

        return NoContent();
    }

    // Delete by id
    [HttpDelete("id/{id}")]
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

        var testCodes = _context.TestCode.Where(tc => tc.authorId == id);
        var exerciseTests = _context.ExerciseTest.Where(et => et.testId == testCodes.FirstOrDefault().id).ToList();
        if (exerciseTests.Any())
            _context.ExerciseTest.RemoveRange(exerciseTests);
            
        if (testCodes.Any())
            _context.TestCode.RemoveRange(testCodes);

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
    }

    //Delete by email
    [HttpDelete("email/{email}")]
    public async Task<IActionResult> DeleteProfileByEmail(string email)
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

        var userElement = usersArray.EnumerateArray()
            .FirstOrDefault(u => u.TryGetProperty("email", out var e) && e.GetString() == email);

        if (userElement.ValueKind == System.Text.Json.JsonValueKind.Undefined)
            return NotFound("No user found with this email in Supabase Auth.");

        var userId = userElement.GetProperty("id").GetString();
        if (!Guid.TryParse(userId, out Guid supabaseUserId))
            return BadRequest("Invalid user ID returned from Supabase.");

        var deleteResponse = await httpClient.DeleteAsync($"/auth/v1/admin/users/{userId}");

        var profile = _context.Profiles.Find(supabaseUserId);

        var achievements = _context.ProfileAchievement.Where(pa => pa.profileId == supabaseUserId);
        if (achievements.Any())
            _context.ProfileAchievement.RemoveRange(achievements);

        var testCodes = _context.TestCode.Where(tc => tc.authorId == supabaseUserId);
        var exerciseTests = _context.ExerciseTest.Where(et => et.testId == testCodes.FirstOrDefault().id).ToList();
        if (exerciseTests.Any())
            _context.ExerciseTest.RemoveRange(exerciseTests);

        if (testCodes.Any())
            _context.TestCode.RemoveRange(testCodes);

        var exercises = _context.ProfileExercise.Where(pe => pe.profileId == supabaseUserId);
        if (exercises.Any())
            _context.ProfileExercise.RemoveRange(exercises);

        var roomUsers = _context.RoomUser.Where(ru => ru.profileId == supabaseUserId);
        if (roomUsers.Any())
            _context.RoomUser.RemoveRange(roomUsers);

        if (profile != null)
            _context.Profiles.Remove(profile);

        if (profile == null && !achievements.Any() && !exercises.Any() && !roomUsers.Any())
            return NotFound();

        _context.SaveChanges();

        return NoContent();
    }
}