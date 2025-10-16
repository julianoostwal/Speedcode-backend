using System.Net.WebSockets;
using System.Text;
using speedCode.Core.utils;
using speedCode.Excise.service;
using speedCode.Profile;
using speedCode.rooms;
using speedCode.services;

namespace speedCode.Excise;

public class RoomWebSocketHandler
{
    private readonly RoomValidator _validator;
    private readonly RoomUserService _roomUserService;
    private readonly ExciseService _exciseService;
    private readonly JwtService _jwtService;

    public RoomWebSocketHandler(RoomValidator validator, RoomUserService roomUserService, ExciseService exciseService)
    {
        _exciseService = exciseService;
        _validator = validator;
        _roomUserService = roomUserService;
    }

    public async Task HandleAsync(Guid roomId, WebSocket socket)
    {
        Guid clientId = await AuthenticateAsync(socket);
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
            FinishedExcise = _exciseService.GetAllFinishedExcisesByRoomAndUserId(roomId, clientId)
        };


        rooms.Room room = WSRoomsService.GetOrCreateRoom(roomId);
        PlayerManager.AddPlayer(room, player);
        _roomUserService.JoinRoom(clientId, roomId);
        await ReceiveLoopAsync(room, player);
    }

    private async Task<Guid> AuthenticateAsync(WebSocket socket)
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
        }

        var token = tokenProp.GetString();
        var clientId = _jwtService.GetClientIdFromToken(token);
        if (string.IsNullOrEmpty(clientId))
        {
            var msg = Encoding.UTF8.GetBytes("Unauthorized");
            await socket.SendAsync(new ArraySegment<byte>(msg), WebSocketMessageType.Text, true,
                CancellationToken.None);
            await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Unauthorized", CancellationToken.None);
        }

        return Guid.Parse(clientId);
    }

    private async Task ReceiveLoopAsync(rooms.Room room, Player player)
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
                    // TODO==: messages
                }
            }
        }
        catch
        {
            PlayerManager.RemovePlayer(room, player.ClientId);
        }
    }
}