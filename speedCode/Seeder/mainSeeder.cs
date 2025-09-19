using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using speedCode.Model;
using speedCode.Data;


namespace speedCode.Seeder
{
    public class MainSeeder
    {
        public static void SeedDatabase(AppDbContext context)
        {
            context.Database.Migrate();

            if (!context.Profiles.Any())
            {
                var profiles = new List<Profiles>
                {
                    new Profiles { Email = "jack@example.com", Password = "Jack123!", AvatarUrl = "", CreatedAt = DateTime.UtcNow, NickName = "Jack" },
                    new Profiles { Email = "nikita@example.com", Password = "Nikita123!", AvatarUrl = "", CreatedAt = DateTime.UtcNow, NickName = "Nikita" },
                    new Profiles { Email = "john@example.com", Password = "John123!", AvatarUrl = "", CreatedAt = DateTime.UtcNow, NickName = "John" },
                    new Profiles { Email = "luuk@example.com", Password = "Luuk123!", AvatarUrl = "", CreatedAt = DateTime.UtcNow, NickName = "Luuk" },
                    new Profiles { Email = "ismael@example.com", Password = "Ismael123!", AvatarUrl = "", CreatedAt = DateTime.UtcNow, NickName = "Ismael" },
                    new Profiles { Email = "denise@example.com", Password = "Denise123!", AvatarUrl = "", CreatedAt = DateTime.UtcNow, NickName = "Denise" }
                };
                context.Profiles.AddRange(profiles);
                context.SaveChanges();
            }

            if (!context.Exercise.Any())
            {
                var exercises = new List<Exercise>
                {
                    new Exercise { title = "Opdracht1", description = "Test1", isRoomExercise = false, CreatedAt = DateTime.UtcNow, points = 10 },
                    new Exercise { title = "Opdracht2", description = "Test2", isRoomExercise = false, CreatedAt = DateTime.UtcNow, points = 5 }
                };
                context.Exercise.AddRange(exercises);
                context.SaveChanges();
            }

            if (!context.Rooms.Any())
            {
                var rooms = new List<Rooms>
                {
                    new Rooms { RoomName = "Room1", RoomLevel = Rooms.Level.Easy, IsPrivate = false, CreatedAt = DateTime.UtcNow },
                    new Rooms { RoomName = "Room2", RoomLevel = Rooms.Level.Medium, IsPrivate = false, CreatedAt = DateTime.UtcNow },
                    new Rooms { RoomName = "Room3", RoomLevel = Rooms.Level.Hard, IsPrivate = false, CreatedAt = DateTime.UtcNow }
                };
                context.Rooms.AddRange(rooms);
                context.SaveChanges();
            }

            /*if (!context.TestCode.Any())
            {
                var testCodes = new List<TestCode>
                {
                    new TestCode { testCode = "Test1", author = , CreatedAt = DateTime.UtcNow },
                    new TestCode { testCode = "Test2", author = , CreatedAt = DateTime.UtcNow }
                };
                context.TestCode.AddRange(testCodes);
                context.SaveChanges();
            }

            if (!context.ExerciseTest.Any())
            {
                var exerciseTests = new List<ExerciseTest>
                {
                    new ExerciseTest { Test = , Exercise =  },
                    new ExerciseTest { Test = , Exercise =  }
                };
                context.ExerciseTest.AddRange(exerciseTests);
                context.SaveChanges();
            }

            if (!context.ProfileExercise.Any())
            {
                var profileExercises = new List<ProfileExercise>
                {
                    new ProfileExercise { Exercise = , Profile =  },
                    new ProfileExercise { Exercise = , Profile =  }
                };
                context.ProfileExercise.AddRange(profileExercises);
                context.SaveChanges();
            }

            if (!context.RoomUser.Any())
            {
                var roomUsers = new List<RoomUser>
                {
                    new RoomUser { Room = , Profile = , CreatedAt = DateTime.UtcNow },
                    new RoomUser { Room = , Profile = , CreatedAt = DateTime.UtcNow }
                };
                context.RoomUser.AddRange(roomUsers);
                context.SaveChanges();
            }

            if (!context.RoomExercise.Any())
            {
                var roomExercises = new List<RoomExercise>
                {
                    new RoomExercise { Room = , Exercise =  },
                    new RoomExercise { Room = , Exercise =  }
                };
                context.RoomExercise.AddRange(roomExercises);
                context.SaveChanges();
            }*/
        }
    }
}