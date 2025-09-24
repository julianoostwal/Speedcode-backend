using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using speedCode.Model;
using speedCode.Data;

// Seeder is nog in test fase

namespace speedCode.Seeder
{
    public class SeederManager
    {
        public static void SeedDatabase(AppDbContext context)
        {
            context.Database.Migrate();

            if (!context.Profiles.Any())
            {
                var profiles = new List<Profiles>
                {
                    new Profiles { AvatarUrl = "", CreatedAt = DateTime.UtcNow, NickName = "Joshua" }/*,
                    new Profiles { AvatarUrl = "", CreatedAt = DateTime.UtcNow, NickName = "Luca" },
                    new Profiles { AvatarUrl = "", CreatedAt = DateTime.UtcNow, NickName = "Zuohuang" },
                    new Profiles { AvatarUrl = "", CreatedAt = DateTime.UtcNow, NickName = "Julian" }*/
                };
                context.Profiles.AddRange(profiles);
                context.SaveChanges();
            }

            if (!context.Exercise.Any())
            {
                var exercises = new List<Exercise>
                {
                    new Exercise { title = "Opdracht1", description = "Test1", isRoomExcise = false, CreatedAt = DateTime.UtcNow, points = 10 },
                    new Exercise { title = "Opdracht2", description = "Test2", isRoomExcise = false, CreatedAt = DateTime.UtcNow, points = 5 }
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

            if (!context.TestCode.Any())
            {
                var testCodes = new List<TestCode>
                {
                    new TestCode { testCode = "TestCode1", authorid = context.Profiles.First().id, CreatedAt = DateTime.UtcNow }/*,
                    new TestCode { testCode = "TestCode2", authorid = context.Profiles.Skip(1).First().id, CreatedAt = DateTime.UtcNow }*/
                };
                context.TestCode.AddRange(testCodes);
                context.SaveChanges();
            }

            if (!context.ExerciseTest.Any())
            {
                var exerciseTests = new List<ExerciseTest>
                {
                    new ExerciseTest { TestId = context.TestCode.First().Id, ExerciseId = context.Exercise.First().Id }/*,
                    new ExerciseTest { TestId = context.TestCode.Skip(1).First().Id, ExerciseId = context.Exercise.First().Id }*/
                };
                context.ExerciseTest.AddRange(exerciseTests);
                context.SaveChanges();
            }

            if (!context.ProfileExercise.Any())
            {
                var profileExercises = new List<ProfileExercise>
                {
                    new ProfileExercise { ExerciseId = context.Exercise.First().Id, Profileid = context.Profiles.First().id }/*,
                    new ProfileExercise { ExerciseId = context.Exercise.First().Id, Profileid = context.Profiles.Skip(1).First().id }*/
                };
                context.ProfileExercise.AddRange(profileExercises);
                context.SaveChanges();
            }

            if (!context.RoomUser.Any())
            {
                var roomUsers = new List<RoomUser>
                {
                    new RoomUser { RoomId = context.Rooms.First().Id, Profileid = context.Profiles.First().id, CreatedAt = DateTime.UtcNow }/*,
                    new RoomUser { RoomId = context.Rooms.First().Id, Profileid = context.Profiles.Skip(1).First().id, CreatedAt = DateTime.UtcNow }*/
                };
                context.RoomUser.AddRange(roomUsers);
                context.SaveChanges();
            }

            if (!context.RoomExercise.Any())
            {
                var roomExercises = new List<RoomExercise>
                {
                    new RoomExercise { RoomId = context.Rooms.First().Id, ExerciseId = context.Exercise.First().Id }/*,
                    new RoomExercise { RoomId = context.Rooms.First().Id, ExerciseId = context.Exercise.Skip(1).First().Id.ToString() }*/
                };
                context.RoomExercise.AddRange(roomExercises);
                context.SaveChanges();
            }
        }
    }
}