using System;
using System.Collections.Generic;
using speedCode.Core.interfaces;

namespace speedCode.Room.dtos;


// todo joshua: hergebruik de class die al bestaat.
public class RoomFullDto
{
    public Guid id { get; set; }
    public string roomName { get; set; }
    public string roomLevel { get; set; }
    public bool isPrivate { get; set; }
    public DateTime createdAt { get; set; }
    public Guid CreatedById { get; set; }
    public ProfileDto CreatedBy { get; set; }
    public List<RoomUserDto> RoomUsers { get; set; }
    public List<RoomExerciseDto> RoomExercises { get; set; }
    
    // WebSocket room status
    public bool IsStarted { get; set; }
    public int ActivePlayers { get; set; }

    public ProfileDto Profile { get; set; }

    public class RoomUserDto
    {
        public Guid id { get; set; }
        public Guid roomId { get; set; }
        public Guid profileId { get; set; }
        public DateTime createdAt { get; set; }
        public ProfileDto profile { get; set; }
    }

    public class RoomExerciseDto
    {
        public Guid id { get; set; }
        public Guid roomId { get; set; }
        public Guid exerciseId { get; set; }
        public ExerciseDto exercise { get; set; }
    }

    public class ExerciseDto
    {
        public Guid id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime createdAt { get; set; }
        public bool isRoomExercise { get; set; }
        public bool isPrivate { get; set; }
        public int points { get; set; }
        public string Level { get; set; }
    }
}