using System.Net.WebSockets;

namespace speedCode.rooms;

public class Player
{
    public string ClientId { get; set; } = "";
    public WebSocket Socket { get; set; } = default!;
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
    public bool IsCompleted { get; set; }
}