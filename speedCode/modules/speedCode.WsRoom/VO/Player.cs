using System.Net.WebSockets;
using speedCode.Profile.Model;

namespace speedCode.rooms;

public class Player
{
    public required Profiles Profile { get; set; }
    public required WebSocket Socket { get; set; }

    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;

    public List<Guid> Finishedexercise { get; set; } = new List<Guid>();
}