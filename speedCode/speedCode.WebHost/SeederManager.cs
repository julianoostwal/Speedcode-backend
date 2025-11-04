using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using speedCode.Core.judge0.dto;
using speedCode.Core.rooms.enums;
using speedCode.exercise;
using speedCode.exercise.model;
using speedCode.Profile;
using speedCode.Profile.model;
using speedCode.Room;
using speedCode.Room.Model;

namespace speedCode.Seeder
{
    public class SeederManager
    {
        // Creates users in Supabase Auth

        public static async Task CreateSupabaseAuthUsersAsync(string supabaseUrl, string serviceRoleKey)
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(supabaseUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", serviceRoleKey);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("apikey", serviceRoleKey);

            var random = new Random();
            for (int i = 0; i < 6; i++)
            {
                var email = $"{RandomString(random, 5)}@example.com";

                var usersResponse = await httpClient.GetAsync($"/auth/v1/admin/users?email={email}");
                var usersJson = await usersResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"User lookup response for {email}: {usersJson}");
                if (usersResponse.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(usersJson);
                    if (doc.RootElement.TryGetProperty("users", out var usersArray) &&
                        usersArray.ValueKind == JsonValueKind.Array)
                    {
                        bool exists = usersArray.EnumerateArray().Any(u =>
                            u.TryGetProperty("email", out var e) && e.GetString() == email
                        );
                        if (exists)
                            continue;
                    }
                }

                var payload = new
                {
                    email,
                    password = "Test123",
                    email_confirm = true
                };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("/auth/v1/admin/users", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to create user {email}: {error}");
                }
                else
                {
                    Console.WriteLine($"Created user {email}");
                }
            }
        }

        // Gets a random string of lowercase letters

        private static string RandomString(Random random, int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static void SeedDatabase(RoomsDb roomsDb, ExerciseDb exerciseDb, ProfileDb profileDb)
        {
            Console.WriteLine("Seeding database...");
            // Seed Exercises

            if (!exerciseDb.Exercise.Any())
            {
                var exercises = new List<Exercise>
                {
                    new Exercise
                    {
                        Level = Level.Easy,
                        title = "Opdracht1", description = "Test1", isRoomexercise = false, createdAt = DateTime.UtcNow,
                        points = 5
                    },
                    new Exercise
                    {
                        Level = Level.Easy,
                        title = "Opdracht2", description = "Test2", isRoomexercise = false, createdAt = DateTime.UtcNow,
                        points = 10
                    },
                    new Exercise
                    {
                        Level = Level.Easy,
                        title = "Opdracht3", description = "Test3", isRoomexercise = false, createdAt = DateTime.UtcNow,
                        points = 15
                    },
                    new Exercise
                    {
                        Level = Level.Easy,
                        title = "Opdracht4", description = "Test4", isRoomexercise = false, createdAt = DateTime.UtcNow,
                        points = 20
                    },
                    new Exercise
                    {
                        Level = Level.Easy,
                        title = "Opdracht5", description = "Test5", isRoomexercise = false, createdAt = DateTime.UtcNow,
                        points = 25
                    },
                    new Exercise
                    {
                        Level = Level.Easy,
                        title = "Opdracht6", description = "Test6", isRoomexercise = false, createdAt = DateTime.UtcNow,
                        points = 30
                    }
                };
                exerciseDb.Exercise.AddRange(exercises);
                exerciseDb.SaveChanges();
                profileDb.SaveChanges();
            }

            // Seed Rooms

            if (!roomsDb.rooms.Any())
            {
                var profile = profileDb.Profiles.AsNoTracking().First();
                var rooms = new List<Rooms>
                {
                    new Rooms
                    {
                        CreatedById = profile.id,
                        roomName = "Room1", roomLevel = Level.Easy, isPrivate = false, createdAt = DateTime.UtcNow
                    },
                    new Rooms
                    {
                        CreatedById = profile.id,
                        roomName = "Room2", roomLevel = Level.Medium, isPrivate = false,
                        createdAt = DateTime.UtcNow
                    },
                    new Rooms
                    {
                        CreatedById = profile.id,
                        roomName = "Room3", roomLevel = Level.Hard, isPrivate = false, createdAt = DateTime.UtcNow
                    }
                };
                roomsDb.rooms.AddRange(rooms);
                roomsDb.SaveChanges();
            }


            // Seed TestCode and testExpectIOs
            if (!exerciseDb.TestCode.Any() && profileDb.Profiles.Any())
            {
                var testCodes = new List<TestCode>();
                int count = profileDb.Profiles.Count();

                var profiles = profileDb.Profiles.ToList();

                for (int i = 0; i < count; i++)
                {
                    var author = profiles[i];

                    var testCode = new TestCode
                    {
                        // AuthorId = author.id,
                        createdAt = DateTime.UtcNow,
                        Type = TestType.Exactly
                    };

                    testCode.TestExpectIOs = new List<TestExpectIO>
                    {
                        new TestExpectIO
                        {
                            input = $"Input for TestCode {i + 1} - Case 1",
                            output = $"Output for TestCode {i + 1} - Case 1"
                        },
                        new TestExpectIO
                        {
                            input = $"Input for TestCode {i + 1} - Case 2",
                            output = $"Output for TestCode {i + 1} - Case 2"
                        }
                    };

                    testCodes.Add(testCode);
                }

                exerciseDb.TestCode.AddRange(testCodes);
                exerciseDb.SaveChanges();
                profileDb.SaveChanges();
            }

            // Seed ExerciseTest

            if (!exerciseDb.ExerciseTest.Any() && exerciseDb.TestCode.Any() && exerciseDb.Exercise.Any())
            {
                var exerciseTests = new List<ExerciseTest>();
                int countExercise = exerciseDb.Exercise.Count();
                int countTestCode = exerciseDb.TestCode.Count();

                for (int i = 0; i < Math.Min(countExercise, countTestCode); i++)
                {
                    exerciseTests.Add(new ExerciseTest
                    {
                        testId = exerciseDb.TestCode.Skip(i).First().id, exerciseId = exerciseDb.Exercise.Skip(i).First().id
                    });
                }

                exerciseDb.ExerciseTest.AddRange(exerciseTests);
                exerciseDb.SaveChanges();
            }

            // Seed ProfileExercise

            if (!profileDb.CompletedProfileExercise.Any() && exerciseDb.Exercise.Any() && profileDb.Profiles.Any())
            {
                var profileExercises = new List<CompletedProfileExercise>();
                int countExercise = exerciseDb.Exercise.Count();
                int countProfiles = profileDb.Profiles.Count();

                for (int i = 0; i < Math.Min(countExercise, countProfiles); i++)
                {
                    profileExercises.Add(new CompletedProfileExercise
                    {
                        exerciseId = exerciseDb.Exercise.Skip(i).First().id,
                        profileId = profileDb.Profiles.Skip(i).First().id
                    });
                }

                profileDb.CompletedProfileExercise.AddRange(profileExercises);
                exerciseDb.SaveChanges();
                profileDb.SaveChanges();
            }

            //Seed createdProfileExercises
            if (!profileDb.CreatedProfileExercise.Any() && exerciseDb.Exercise.Any() && profileDb.Profiles.Any())
            {
                var profileExercises = new List<CreatedProfileExercises>();
                int countExercise = exerciseDb.Exercise.Count();
                int countProfiles = profileDb.Profiles.Count();

                for (int i = 0; i < Math.Min(countExercise, countProfiles); i++)
                {
                    profileExercises.Add(new CreatedProfileExercises()
                    {
                        exerciseId = exerciseDb.Exercise.Skip(i).First().id,
                        profileId = profileDb.Profiles.Skip(i).First().id
                    });
                }

                profileDb.CreatedProfileExercise.AddRange(profileExercises);
                exerciseDb.SaveChanges();
                profileDb.SaveChanges();
            }


            // Seed RoomUser

            if (!roomsDb.RoomUser.Any() && roomsDb.rooms.Any() && profileDb.Profiles.Any())
            {
                var roomUsers = new List<RoomUser>();
                int countRooms = roomsDb.rooms.Count();
                int countProfiles = profileDb.Profiles.Count();

                for (int roomIdx = 0; roomIdx < countRooms; roomIdx++)
                {
                    for (int profileIdx = 0; profileIdx < 2; profileIdx++)
                    {
                        int globalProfileIdx = roomIdx * 2 + profileIdx;
                        if (globalProfileIdx >= countProfiles) break;

                        roomUsers.Add(new RoomUser
                        {
                            roomId = roomsDb.rooms.Skip(roomIdx).First().id,
                            profileId = profileDb.Profiles.Skip(globalProfileIdx).First().id,
                            createdAt = DateTime.UtcNow
                        });
                    }
                }

                roomsDb.RoomUser.AddRange(roomUsers);
                roomsDb.SaveChanges();
                profileDb.SaveChanges();
            }

            // Seed RoomExercise

            if (!roomsDb.RoomExercise.Any() && roomsDb.rooms.Any() && exerciseDb.Exercise.Any())
            {
                var roomExercises = new List<RoomExercise>();
                int countRooms = roomsDb.rooms.Count();
                int countExercises = exerciseDb.Exercise.Count();

                for (int roomIdx = 0; roomIdx < countRooms; roomIdx++)
                {
                    for (int exerciseIdx = 0; exerciseIdx < 2; exerciseIdx++)
                    {
                        int globalExerciseIdx = roomIdx * 2 + exerciseIdx;
                        if (globalExerciseIdx >= countExercises) break;

                        roomExercises.Add(new RoomExercise
                        {
                            roomId = roomsDb.rooms.Skip(roomIdx).First().id,
                            exerciseId = exerciseDb.Exercise.Skip(globalExerciseIdx).First().id
                        });
                    }
                }

                roomsDb.RoomExercise.AddRange(roomExercises);
                exerciseDb.SaveChanges();
                roomsDb.SaveChanges();
            }

            // Seed Achievements

            if (!profileDb.Achievement.Any())
            {
                var Achievement = new List<Achievement>
                {
                    new Achievement
                    {
                        achievementIcon = "🏆", achievementName = "Speed Demon",
                        achievementDescription = "Achieved 90+ WPM"
                    },
                    new Achievement
                    {
                        achievementIcon = "🎯", achievementName = "Precision Master",
                        achievementDescription = "99% accuracy in a race"
                    },
                    new Achievement
                    {
                        achievementIcon = "🔥", achievementName = "Hot Streak",
                        achievementDescription = "10 day coding streak"
                    },
                    new Achievement
                    {
                        achievementIcon = "⚡", achievementName = "Lightning Fast",
                        achievementDescription = "Complete race under 2 minutes"
                    },
                    new Achievement
                    {
                        achievementIcon = "🌟", achievementName = "Rising Star",
                        achievementDescription = "Top 100 this month"
                    },
                    new Achievement
                    {
                        achievementIcon = "💎", achievementName = "Diamond Coder",
                        achievementDescription = "500 races completed"
                    }
                };
                profileDb.Achievement.AddRange(Achievement);
                profileDb.SaveChanges();
            }

            Console.WriteLine("Seeding completed.");
        }
    }
}