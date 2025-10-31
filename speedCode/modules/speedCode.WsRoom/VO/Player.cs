using System.Net.WebSockets;

namespace speedCode.rooms;

public class Player
{
    public Guid ClientId { get; set; }
    public WebSocket Socket { get; set; } = default!;
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;

    public List<Guid> FinishedExcise { get; set; } = new List<Guid>();
}