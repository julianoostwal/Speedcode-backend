using System.Net.WebSockets;
using System.Text;
using speedCode.security;

namespace speedCode.rooms;

public class WebSocketHandler
{
    private readonly RoomValidator _validator;

    public WebSocketHandler(RoomValidator validator)
    {
        _validator = validator;
    }

    public async Task HandleAsync(string roomId, WebSocket socket)
    {
        var clientId = await AuthenticateAsync(socket);
        if (string.IsNullOrEmpty(clientId)) return;

        if (!await _validator.RoomExistsAsync(roomId) || !await _validator.ClientExistsAsync(clientId))
        {
            var msg = Encoding.UTF8.GetBytes("Room or client not found");
            await socket.SendAsync(new ArraySegment<byte>(msg), WebSocketMessageType.Text, true,
                CancellationToken.None);
            await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Room or client not found",
                CancellationToken.None);
            return;
        }

        var player = new Player
        {
            ClientId = clientId,
            Socket = socket,
            ConnectedAt = DateTime.UtcNow,
        };


        Room room = WSRoomsService.GetOrCreateRoom(roomId);
        PlayerManager.AddPlayer(room, player);
        await ReceiveLoopAsync(room, player);
    }

    private async Task<string?> AuthenticateAsync(WebSocket socket)
    {
        var buffer = new byte[1024 * 4];
        var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        var json = System.Text.Json.JsonDocument.Parse(message);

        var root = json.RootElement;

        bool hasType = root.TryGetProperty("type", out var typeProp);
        bool hasToken = root.TryGetProperty("token", out var tokenProp);
        string? type = hasType ? typeProp.GetString() : null;

        if (type != "auth" || !hasToken)
        {
            var msg = Encoding.UTF8.GetBytes("Missing token");
            await socket.SendAsync(new ArraySegment<byte>(msg), WebSocketMessageType.Text, true,
                CancellationToken.None);
            await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Missing token", CancellationToken.None);
            return null;
        }

        var token = tokenProp.GetString();
        var clientId = AuthManager.GetClientIdFromToken(token);
        if (string.IsNullOrEmpty(clientId))
        {
            var msg = Encoding.UTF8.GetBytes("Unauthorized");
            await socket.SendAsync(new ArraySegment<byte>(msg), WebSocketMessageType.Text, true,
                CancellationToken.None);
            await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Unauthorized", CancellationToken.None);
        }

        return clientId;
    }

    private async Task ReceiveLoopAsync(Room room, Player player)
    {
        var buffer = new byte[1024 * 4];
        await room.BroadcastRoomUpdateAsync();

        try
        {
            while (player.Socket.State == WebSocketState.Open)
            {
                var result = await player.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    PlayerManager.RemovePlayer(room, player.ClientId);
                    await player.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed",
                        CancellationToken.None);
                    break;
                }

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    // TODO: messages
                }
            }
        }
        catch
        {
            PlayerManager.RemovePlayer(room, player.ClientId);
        }
    }
}