namespace speedCode.dto.room
{
    public class UpdateRoomStateDto
    {
        public string Type { get; } = "room_state";
        public List<PlayerStateDto> Players { get; set; } = new();
    }

    public class PlayerStateDto
    {
        public Guid ClientId { get; set; }
        public required List<Guid> FinishedExcise { get; set; }
    }
}