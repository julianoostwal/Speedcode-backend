using System;
using System.Collections.Generic;

namespace speedCode.Room.dtos;

public class LeaderboardDto
{
    public Guid id { get; set; }
    public string roomName { get; set; }
    public int totalPoints { get; set; }
    public TimeSpan totalTime { get; set; }
}