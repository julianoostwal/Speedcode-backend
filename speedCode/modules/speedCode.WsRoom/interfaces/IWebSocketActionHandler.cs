using System.Net.WebSockets;
using System.Text.Json;

namespace speedCode.WsRoom.interfaces;

public interface IWebSocketActionHandler
{
    string Action { get; }
    Task HandleAsync(WebSocket socket, JsonElement payload, Guid roomId);
}