using Microsoft.AspNetCore.Mvc;
using speedCode.Data;
using speedCode.Model;
using speedCode.dto;

namespace speedCode.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExercisesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExercisesController(AppDbContext context)
    {
        _context = context;
    }

    // GET all exercises
    [HttpGet]
    public async Task<IActionResult> GetFullExercises()
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

        var exercises = _context.Exercise.ToList();
        var exerciseTests = _context.ExerciseTest.ToList();
        var testCodes = _context.TestCode.ToList();
        var profileExercises = _context.ProfileExercise.ToList();
        var profiles = _context.Profiles.ToList();
        var roomExercises = _context.RoomExercise.ToList();
        var rooms = _context.Rooms.ToList();

        var result = new List<object>();

        var userEmailDict = usersArray.EnumerateArray()
            .Where(u => u.TryGetProperty("id", out var idProp) && u.TryGetProperty("email", out var emailProp))
            .ToDictionary(
                u => u.GetProperty("id").GetString(),
                u => u.GetProperty("email").GetString()
            );

        foreach (var exercise in exercises)
        {
            var profileExerciseList = profileExercises.Where(pe => pe.exerciseId == exercise.id).ToList();

            var exerciseProfiles = profileExerciseList
                .Select(pe =>
                {
                    var profile = profiles.FirstOrDefault(p => p.id == pe.profileId);
                    var profileIdStr = pe.profileId.ToString();
                    var email = userEmailDict.ContainsKey(profileIdStr) ? userEmailDict[profileIdStr] : null;

                    return new
                    {
                        id = profileIdStr,
                        email,
                        avatarUrl = profile?.avatarUrl,
                        createdAt = profile?.createdAt,
                        nickName = profile?.nickName,
                        role = profile?.role.ToString() ?? "user"
                    };
                }).ToList();

            result.Add(new
            {
                id = exercise.id,
                title = exercise.title,
                description = exercise.description,
                isRoomExcise = exercise.isRoomExcise,
                points = exercise.points,
                createdAt = exercise.createdAt,
                profiles = exerciseProfiles
            });
        }
        return Ok(result);
    }

    // GET by id
    /*[HttpGet("id/{id}")]
    public async Task<IActionResult> GetExercise(Guid id)
    {
        var exercise = _context.Exercise.Find(id);
        if (exercise == null)
            return NotFound("Exercise not found");

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

        var exercises = _context.Exercise.Where(e => e.id  == id).ToList();
        var exerciseTests = _context.ExerciseTest.Where(et => et.exerciseId == id).ToList();
        var testCodes = _context.TestCode.Where(tc => tc.id == exerciseTests.FirstOrDefault().testId).ToList();
        var profileExercises = _context.ProfileExercise.Where(pe => pe.exerciseId == id).ToList();
        var roomExercises = _context.RoomExercise.Where(re => re.exerciseId == id).ToList();
        var rooms = _context.Rooms.Where(r => r.id == roomExercises.FirstOrDefault().roomId).ToList();
        var profiles = _context.Profiles.Where(p => p.id == profileExercises.FirstOrDefault().profileId).ToList();
        
        var exerciseProfiles = testCodes
                .Select(pe =>
                {
                    var profile = profiles.FirstOrDefault(p => p.id == pe.authorId);
                    var userElement = usersArray.EnumerateArray()
                        .FirstOrDefault(u => u.GetProperty("id").GetString() == pe.authorId.ToString());
                    var email = userElement.ValueKind != System.Text.Json.JsonValueKind.Undefined
                        ? userElement.GetProperty("email").GetString()
                        : null;

                    return new
                    {
                        id = pe.authorId.ToString(),
                        email,
                        avatarUrl = profile?.avatarUrl,
                        createdAt = profile?.createdAt,
                        nickName = profile?.nickName,
                        role = profile?.role.ToString() ?? "user"
                    };
                }).ToList();

        var result = new List<object>();

        result.Add(new
        {
            id = exercise.id,
            title = exercise.title,
            description = exercise.description,
            isRoomExcise = exercise.isRoomExcise,
            points = exercise.points,
            createdAt = exercise.createdAt,
            exerciseTests = exerciseTests.Select(et => new
            {
                id = et.id,
                exerciseId = et.exerciseId,
                testId = et.testId
            }),
            testCodes = testCodes.Select(tc => new
            {
                id = tc.id,
                authorId = tc.authorId,
                expectedResult = tc.expectedResult,
                type = tc.Type.ToString(),
                createdAt = tc.createdAt
            }),
            profiles = exerciseProfiles,
            profileExercises = profileExercises.Select(pe => new
            {
                id = pe.id,
                profileId = pe.profileId,
                exerciseId = pe.exerciseId
            }),
            roomExercises = roomExercises.Select(re => new
            {
                id = re.id,
                roomId = re.roomId,
                exerciseId = re.exerciseId
            }),
            rooms = rooms.Select(r => new
            {
                id = r.id,
                title = r.title,
                description = r.description,
                createdAt = r.createdAt
            })
        });

        return Ok(result);
    }

    // POST
    /*[HttpPost]
    public async Task<IActionResult> CreateExercise([FromBody] ExerciseRequest request)
    {
        
    }

    // Update user by id
    /*[HttpPut("id/{id}")]
    public async Task<IActionResult> UpdateExercise(Guid id, [FromBody] UpdateExerciseRequest updatedExercise)
    {

    }

    // Delete by id
    /*[HttpDelete("id/{id}")]
    public async Task<IActionResult> DeleteExercise(Guid id)
    {
        
    }*/
}