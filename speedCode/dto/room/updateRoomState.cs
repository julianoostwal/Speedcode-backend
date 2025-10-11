namespace speedCode.dto.room
{
    public class UpdateRoomStateDto
    {
        public string Type { get; } = "room_state";
        public List<PlayerStateDto> Players { get; set; } = new();
    }

    public class PlayerStateDto
    {
        public string ClientId { get; set; } = string.Empty;
        public required List<Guid> FinishedExcise { get; set; }
    }
}