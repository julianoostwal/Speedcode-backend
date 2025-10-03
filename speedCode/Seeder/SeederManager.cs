using Microsoft.EntityFrameworkCore;
using speedCode.Model;
using speedCode.Data;
using speedCode.dto;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
                    if (doc.RootElement.TryGetProperty("users", out var usersArray) && usersArray.ValueKind == JsonValueKind.Array)
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

        public static void SeedDatabase(AppDbContext context)
        {
            context.Database.Migrate();

            // Seed Exercises

            if (!context.Exercise.Any())
            {
                var exercises = new List<Exercise>
                {
                    new Exercise
                    {
                        title = "Opdracht1", description = "Test1", isRoomExcise = false, createdAt = DateTime.UtcNow,
                        points = 5
                    },
                    new Exercise
                    {
                        title = "Opdracht2", description = "Test2", isRoomExcise = false, createdAt = DateTime.UtcNow,
                        points = 10
                    },
                    new Exercise
                    {
                        title = "Opdracht3", description = "Test3", isRoomExcise = false, createdAt = DateTime.UtcNow,
                        points = 15
                    },
                    new Exercise
                    {
                        title = "Opdracht4", description = "Test4", isRoomExcise = false, createdAt = DateTime.UtcNow,
                        points = 20
                    },
                    new Exercise
                    {
                        title = "Opdracht5", description = "Test5", isRoomExcise = false, createdAt = DateTime.UtcNow,
                        points = 25
                    },
                    new Exercise
                    {
                        title = "Opdracht6", description = "Test6", isRoomExcise = false, createdAt = DateTime.UtcNow,
                        points = 30
                    }
                };
                context.Exercise.AddRange(exercises);
                context.SaveChanges();
            }

            // Seed Rooms

            if (!context.Rooms.Any())
            {
                var rooms = new List<Rooms>
                {
                    new Rooms
                    {
                        roomName = "Room1", roomLevel = Rooms.Level.Easy, isPrivate = false, createdAt = DateTime.UtcNow
                    },
                    new Rooms
                    {
                        roomName = "Room2", roomLevel = Rooms.Level.Medium, isPrivate = false, createdAt = DateTime.UtcNow
                    },
                    new Rooms
                    {
                        roomName = "Room3", roomLevel = Rooms.Level.Hard, isPrivate = false, createdAt = DateTime.UtcNow
                    }
                };
                context.Rooms.AddRange(rooms);
                context.SaveChanges();
            }

            // Seed TestCode

            if (!context.TestCode.Any() && context.Profiles.Any())
            {
                var testCodes = new List<TestCode>();
                int count = context.Profiles.Count();

                for (int i = 0; i < count; i++)
                {
                    testCodes.Add(new TestCode { expectedResult = new[] { $"TestCode{i + 1}" }, authorId = context.Profiles.Skip(i).First().id, createdAt = DateTime.UtcNow, Type = TestType.Exactly });
                }
                context.TestCode.AddRange(testCodes);
                context.SaveChanges();
            }

            // Seed ExerciseTest

            if (!context.ExerciseTest.Any() && context.TestCode.Any() && context.Exercise.Any())
            {
                var exerciseTests = new List<ExerciseTest>();
                int countExercise = context.Exercise.Count();
                int countTestCode = context.TestCode.Count();

                for (int i = 0; i < Math.Min(countExercise, countTestCode); i++)
                {
                    exerciseTests.Add(new ExerciseTest { testId = context.TestCode.Skip(i).First().id, exerciseId = context.Exercise.Skip(i).First().id });
                }
                context.ExerciseTest.AddRange(exerciseTests);
                context.SaveChanges();
            }

            // Seed ProfileExercise

            if (!context.ProfileExercise.Any() && context.Exercise.Any() && context.Profiles.Any())
            {
                var profileExercises = new List<ProfileExercise>();
                int countExercise = context.Exercise.Count();
                int countProfiles = context.Profiles.Count();

                for (int i = 0; i < Math.Min(countExercise, countProfiles); i++)
                {
                    profileExercises.Add(new ProfileExercise { exerciseId = context.Exercise.Skip(i).First().id, profileId = context.Profiles.Skip(i).First().id });
                }
                context.ProfileExercise.AddRange(profileExercises);
                context.SaveChanges();
            }

            // Seed RoomUser

            if (!context.RoomUser.Any() && context.Rooms.Any() && context.Profiles.Any())
            {
                var roomUsers = new List<RoomUser>();
                int countRooms = context.Rooms.Count();
                int countProfiles = context.Profiles.Count();

                for (int roomIdx = 0; roomIdx < countRooms; roomIdx++)
                {
                    for (int profileIdx = 0; profileIdx < 2; profileIdx++)
                    {
                        int globalProfileIdx = roomIdx * 2 + profileIdx;
                        if (globalProfileIdx >= countProfiles) break;

                        roomUsers.Add(new RoomUser
                        {
                            roomId = context.Rooms.Skip(roomIdx).First().id,
                            profileId = context.Profiles.Skip(globalProfileIdx).First().id,
                            createdAt = DateTime.UtcNow
                        });
                    }
                }
                context.RoomUser.AddRange(roomUsers);
                context.SaveChanges();
            }

            // Seed RoomExercise

            if (!context.RoomExercise.Any() && context.Rooms.Any() && context.Exercise.Any())
            {
                var roomExercises = new List<RoomExercise>();
                int countRooms = context.Rooms.Count();
                int countExercises = context.Exercise.Count();

                for (int roomIdx = 0; roomIdx < countRooms; roomIdx++)
                {
                    for (int exerciseIdx = 0; exerciseIdx < 2; exerciseIdx++)
                    {
                        int globalExerciseIdx = roomIdx * 2 + exerciseIdx;
                        if (globalExerciseIdx >= countExercises) break;

                        roomExercises.Add(new RoomExercise
                        {
                            roomId = context.Rooms.Skip(roomIdx).First().id,
                            exerciseId = context.Exercise.Skip(globalExerciseIdx).First().id
                        });
                    }
                }

                context.RoomExercise.AddRange(roomExercises);
                context.SaveChanges();
            }

            // Seed Achievements

            if (!context.Achievement.Any())
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
                context.Achievement.AddRange(Achievement);
                context.SaveChanges();
            }

            // Seed ContactInfo

            if (!context.ContactInfo.Any())
            {
                var contactInfos = new List<ContactInfo>
                {
                    new ContactInfo
                    {
                        city = "Purmerend",
                        address = "SpinneKop 2",
                        postalCode = "1444 GN",
                        emailAddress = "john.doe@example.com"
                    }
                };
                context.ContactInfo.AddRange(contactInfos);
                context.SaveChanges();
            }

            // Seed SocialLinks

            if (!context.SocialLink.Any())
            {
                var socialLinks = new List<SocialLink>
                {
                    new SocialLink
                    {
                        platform = "Github",
                        href = "#",
                        icon = ""
                    }
                };
                context.SocialLink.AddRange(socialLinks);
                context.SaveChanges();
            }

            // Seed DropdownItems

            if (!context.DropdownItems.Any())
            {
                var dropdownItems = new List<DropdownItems>
                {
                    new DropdownItems
                    {
                        label = "Dropdown Item",
                        href = "#",
                        description = "Description for dropdown item"
                    }
                };
                context.DropdownItems.AddRange(dropdownItems);
                context.SaveChanges();
            }

            // Seed NavItems

            if (!context.NavItem.Any())
            {
                var navItems = new List<NavItem>
                {
                    new NavItem
                    {
                        label = "Nav Item",
                        href = "#",
                        isDropdown = false,
                        dropdownItemsId = context.DropdownItems.First().id
                    }
                };
                context.NavItem.AddRange(navItems);
                context.SaveChanges();
            }

            // Seed FooterSectionsLinks

            if (!context.FooterSectionLinks.Any())
            {
                var footerSectionLinks = new List<FooterSectionLinks>
                {
                    new FooterSectionLinks
                    {
                        label = "Footer Link",
                        href = "#",
                        isExternal = false
                    }
                };
                context.FooterSectionLinks.AddRange(footerSectionLinks);
                context.SaveChanges();
            }

            // Seed FooterSections

            if (!context.FooterSection.Any())
            {
                var footerSections = new List<FooterSection>
                {
                    new FooterSection
                    {
                        title = "Footer section",
                        linksId = context.FooterSectionLinks.First().id
                    }
                };
                context.FooterSection.AddRange(footerSections);
                context.SaveChanges();
            }
        }
    }
}