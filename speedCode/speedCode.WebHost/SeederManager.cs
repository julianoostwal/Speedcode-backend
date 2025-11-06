using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using speedCode.Core.judge0.dto;
using speedCode.Core.rooms.enums;
using speedCode.exercise;
using speedCode.Exercise;
using speedCode.Exercise.model;
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
            
            // Seed Exercises met realistische programmeer opdrachten
            if (!exerciseDb.Exercise.Any())
            {
                var exercises = new List<Exercise.model.Exercise>
                {
                    new Exercise.model.Exercise
                    {
                        Level = Level.Easy,
                        title = "Hello World",
                        description = "Schrijf een programma dat 'Hello, World!' print naar de console.",
                        isRoomExercise = false,
                        isPrivate = false,
                        createdAt = DateTime.UtcNow,
                        points = 5
                    },
                    new Exercise.model.Exercise
                    {
                        Level = Level.Easy,
                        title = "Som van Twee Getallen",
                        description = "Schrijf een programma dat twee getallen inleest en de som ervan print.",
                        isRoomExercise = false,
                        isPrivate = false,
                        createdAt = DateTime.UtcNow,
                        points = 10
                    },
                    new Exercise.model.Exercise
                    {
                        Level = Level.Easy,
                        title = "Even of Oneven",
                        description = "Schrijf een programma dat controleert of een getal even of oneven is en print 'even' of 'oneven'.",
                        isRoomExercise = false,
                        isPrivate = false,
                        createdAt = DateTime.UtcNow,
                        points = 10
                    },
                    new Exercise.model.Exercise
                    {
                        Level = Level.Medium,
                        title = "FizzBuzz",
                        description = "Print getallen van 1 tot 15. Voor veelvouden van 3 print 'Fizz', voor veelvouden van 5 print 'Buzz', voor veelvouden van beide print 'FizzBuzz'.",
                        isRoomExercise = false,
                        isPrivate = false,
                        createdAt = DateTime.UtcNow,
                        points = 20
                    },
                    new Exercise.model.Exercise
                    {
                        Level = Level.Medium,
                        title = "Palindroom Checker",
                        description = "Schrijf een programma dat controleert of een woord een palindroom is. Print 'true' of 'false'.",
                        isRoomExercise = false,
                        isPrivate = false,
                        createdAt = DateTime.UtcNow,
                        points = 25
                    },
                    new Exercise.model.Exercise
                    {
                        Level = Level.Hard,
                        title = "Fibonacci Reeks",
                        description = "Genereer de eerste N getallen van de Fibonacci reeks, gescheiden door spaties.",
                        isRoomExercise = false,
                        isPrivate = false,
                        createdAt = DateTime.UtcNow,
                        points = 35
                    },
                    new Exercise.model.Exercise
                    {
                        Level = Level.Hard,
                        title = "Priemgetal Checker",
                        description = "Schrijf een programma dat alle priemgetallen tot en met N print, gescheiden door spaties.",
                        isRoomExercise = false,
                        isPrivate = false,
                        createdAt = DateTime.UtcNow,
                        points = 40
                    }
                };
                exerciseDb.Exercise.AddRange(exercises);
                exerciseDb.SaveChanges();
            }

            // Seed Rooms

            if (!roomsDb.rooms.Any() && profileDb.Profiles.Any())
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


            // Seed TestCode met realistische test cases
            if (!exerciseDb.TestCode.Any())
            {
                var exercises = exerciseDb.Exercise.OrderBy(e => e.createdAt).ToList();
                var testCodes = new List<TestCode>();

                if (exercises.Count > 0)
                {
                    // Test 1: Hello World
                    testCodes.Add(new TestCode
                    {
                        LanguageId = 71,
                        createdAt = DateTime.UtcNow,
                        Type = TestType.Exactly,
                        ACRegex = @"print\s*\(\s*[""']Hello,\s*World![""']\s*\)",
                        TestExpectIOs = new List<TestExpectIO>
                        {
                            new TestExpectIO
                            {
                                input = "",
                                output = "Hello, World!"
                            }
                        }
                    });

                    // Test 2: Som van Twee Getallen
                    testCodes.Add(new TestCode
                    {
                        LanguageId = 71,
                        createdAt = DateTime.UtcNow,
                        Type = TestType.Exactly,
                        ACRegex = @"(input\s*\(\s*\).*input\s*\(\s*\)|int\s*\(.*int\s*\()",
                        TestExpectIOs = new List<TestExpectIO>
                        {
                            new TestExpectIO
                            {
                                input = "5\n3",
                                output = "8"
                            },
                            new TestExpectIO
                            {
                                input = "10\n20",
                                output = "30"
                            },
                            new TestExpectIO
                            {
                                input = "-5\n5",
                                output = "0"
                            }
                        }
                    });

                    // Test 3: Even of Oneven
                    testCodes.Add(new TestCode
                    {
                        LanguageId = 71,
                        createdAt = DateTime.UtcNow,
                        Type = TestType.Exactly,
                        ACRegex = @"(%|mod)",
                        TestExpectIOs = new List<TestExpectIO>
                        {
                            new TestExpectIO
                            {
                                input = "4",
                                output = "even"
                            },
                            new TestExpectIO
                            {
                                input = "7",
                                output = "oneven"
                            },
                            new TestExpectIO
                            {
                                input = "0",
                                output = "even"
                            }
                        }
                    });

                    // Test 4: FizzBuzz
                    testCodes.Add(new TestCode
                    {
                        LanguageId = 71,
                        createdAt = DateTime.UtcNow,
                        Type = TestType.Exactly,
                        ACRegex = @"(for|while).*range\s*\(.*15",
                        TestExpectIOs = new List<TestExpectIO>
                        {
                            new TestExpectIO
                            {
                                input = "",
                                output = "1\n2\nFizz\n4\nBuzz\nFizz\n7\n8\nFizz\nBuzz\n11\nFizz\n13\n14\nFizzBuzz"
                            }
                        }
                    });

                    // Test 5: Palindroom Checker
                    testCodes.Add(new TestCode
                    {
                        LanguageId = 71,
                        createdAt = DateTime.UtcNow,
                        Type = TestType.Exactly,
                        ACRegex = @"\[::-1\]",
                        TestExpectIOs = new List<TestExpectIO>
                        {
                            new TestExpectIO
                            {
                                input = "radar",
                                output = "true"
                            },
                            new TestExpectIO
                            {
                                input = "hello",
                                output = "false"
                            },
                            new TestExpectIO
                            {
                                input = "level",
                                output = "true"
                            }
                        }
                    });

                    // Test 6: Fibonacci Reeks
                    testCodes.Add(new TestCode
                    {
                        LanguageId = 71,
                        createdAt = DateTime.UtcNow,
                        Type = TestType.Exactly,
                        ACRegex = @"(for|while).*range",
                        TestExpectIOs = new List<TestExpectIO>
                        {
                            new TestExpectIO
                            {
                                input = "5",
                                output = "0 1 1 2 3"
                            },
                            new TestExpectIO
                            {
                                input = "8",
                                output = "0 1 1 2 3 5 8 13"
                            },
                            new TestExpectIO
                            {
                                input = "1",
                                output = "0"
                            }
                        }
                    });

                    // Test 7: Priemgetal Checker
                    testCodes.Add(new TestCode
                    {
                        LanguageId = 71,
                        createdAt = DateTime.UtcNow,
                        Type = TestType.Exactly,
                        ACRegex = @"(for|while).*range.*\(2,",
                        TestExpectIOs = new List<TestExpectIO>
                        {
                            new TestExpectIO
                            {
                                input = "10",
                                output = "2 3 5 7"
                            },
                            new TestExpectIO
                            {
                                input = "20",
                                output = "2 3 5 7 11 13 17 19"
                            },
                            new TestExpectIO
                            {
                                input = "2",
                                output = "2"
                            }
                        }
                    });
                }

                exerciseDb.TestCode.AddRange(testCodes);
                exerciseDb.SaveChanges();
            }

            // Seed ExerciseTest - koppel exercises aan hun tests
            if (!exerciseDb.ExerciseTest.Any() && exerciseDb.TestCode.Any() && exerciseDb.Exercise.Any())
            {
                var exerciseTests = new List<ExerciseTest>();
                var exercises = exerciseDb.Exercise.OrderBy(e => e.createdAt).ToList();
                var testCodes = exerciseDb.TestCode.OrderBy(t => t.createdAt).ToList();

                // Koppel elke oefening aan zijn bijbehorende test
                for (int i = 0; i < Math.Min(exercises.Count, testCodes.Count); i++)
                {
                    exerciseTests.Add(new ExerciseTest
                    {
                        testId = testCodes[i].id,
                        exerciseId = exercises[i].id
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