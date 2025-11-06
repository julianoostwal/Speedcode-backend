using speedCode.Profile.Model;

namespace speedCode.WsRoom
{
    public class UpdateRoomStateDto
    {
        public string Type { get; } = "room_state";
        public List<PlayerStateDto> Players { get; set; } = new();
    }

    public class PlayerStateDto
    {
        public Profiles Profiles { get; set; }
        public required List<Guid> Finishedexercise { get; set; }
    }
}