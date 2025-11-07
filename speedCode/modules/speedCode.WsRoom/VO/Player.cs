using System.Net.WebSockets;
using speedCode.Core.interfaces;

namespace speedCode.rooms;

public class Player
{
    public required ProfileDto Profile { get; set; }
    public required WebSocket Socket { get; set; }

    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;

    public List<Guid> Finishedexercise { get; set; } = new List<Guid>();
}