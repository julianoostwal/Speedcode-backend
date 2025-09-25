using Microsoft.EntityFrameworkCore;
using speedCode.Model;
using speedCode.Data;

namespace speedCode.Seeder
{
    public class SeederManager
    {
        public static void SeedDatabase(AppDbContext context)
        {
            context.Database.Migrate();

            if (!context.Exercise.Any())
            {
                var exercises = new List<Exercise>
                {
                    new Exercise
                    {
                        title = "Opdracht1", description = "Test1", isRoomExcise = false, createdAt = DateTime.UtcNow,
                        points = 10
                    },
                    new Exercise
                    {
                        title = "Opdracht2", description = "Test2", isRoomExcise = false, createdAt = DateTime.UtcNow,
                        points = 5
                    }
                };
                context.Exercise.AddRange(exercises);
                context.SaveChanges();
            }

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
                        roomName = "Room2", roomLevel = Rooms.Level.Medium, isPrivate = false,
                        createdAt = DateTime.UtcNow
                    },
                    new Rooms
                    {
                        roomName = "Room3", roomLevel = Rooms.Level.Hard, isPrivate = false, createdAt = DateTime.UtcNow
                    }
                };
                context.Rooms.AddRange(rooms);
                context.SaveChanges();
            }

            if (!context.TestCode.Any())
            {
                var testCodes = new List<TestCode>
                {
                    new TestCode
                    {
                        ExpectedResult = new[] { "TestCode1" }, authorId = context.Profiles.First().id,
                        CreatedAt = DateTime.UtcNow
                    }
                };
                context.TestCode.AddRange(testCodes);
                context.SaveChanges();
            }

            if (!context.ExerciseTest.Any())
            {
                var exerciseTests = new List<ExerciseTest>
                {
                    new ExerciseTest { testId = context.TestCode.First().id, exerciseId = context.Exercise.First().id }
                };
                context.ExerciseTest.AddRange(exerciseTests);
                context.SaveChanges();
            }

            if (!context.ProfileExercise.Any())
            {
                var profileExercises = new List<ProfileExercise>
                {
                    new ProfileExercise
                        { exerciseId = context.Exercise.First().id, profileId = context.Profiles.First().id }
                };
                context.ProfileExercise.AddRange(profileExercises);
                context.SaveChanges();
            }

            if (!context.RoomUser.Any())
            {
                var roomUsers = new List<RoomUser>
                {
                    new RoomUser
                    {
                        roomId = context.Rooms.First().id, profileId = context.Profiles.First().id,
                        createdAt = DateTime.UtcNow
                    }
                };
                context.RoomUser.AddRange(roomUsers);
                context.SaveChanges();
            }

            if (!context.RoomExercise.Any())
            {
                var roomExercises = new List<RoomExercise>
                {
                    new RoomExercise { roomId = context.Rooms.First().id, exerciseId = context.Exercise.First().id }
                };
                context.RoomExercise.AddRange(roomExercises);
                context.SaveChanges();
            }
        }
    }
}