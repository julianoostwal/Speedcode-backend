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
    public List<ExerciseTestDto> ExerciseTests { get; set; }

    public class ExerciseTestDto
    {
        public Guid id { get; set; }
        public Guid exerciseId { get; set; }
        public Guid testId { get; set; }
        public TestDto test { get; set; }
    }

    public class TestDto
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string acRegex { get; set; }
        public ProfilesDto profilesId { get; set; }
        public List<TestExpectIODto> TestExpectIOs { get; set; }
    }

    public class ProfilesDto
    {
        public Guid id { get; set; }
        public string? avatarUrl { get; set; }
        public DateTime createdAt { get; set; }
        public string? nickName { get; set; }
        public string role { get; set; }
    }

    public class TestExpectIODto
    {
        public Guid id { get; set; }
        public Guid testId { get; set; }
        public string input { get; set; }
        public string output { get; set; }
    }
}