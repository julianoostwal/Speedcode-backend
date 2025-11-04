using System;
using System.Collections.Generic;
using speedCode.Excise.model;

namespace speedCode.Excise.dtos;

public class ExerciseFullDto
{
    public Guid id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public bool isRoomExcise { get; set; }
    public bool isPrivate { get; set; }
    public int points { get; set; }
    public string level { get; set; }
    public DateTime createdAt { get; set; }
}