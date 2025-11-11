using System;
using System.Collections.Generic;
using speedCode.Core.interfaces;
using speedCode.Room.dtos;  

namespace speedCode.Room.dtos;

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
}