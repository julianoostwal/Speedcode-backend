using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using speedCode.Core.utils;
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

    public RoomWebSocketHandler(RoomValidator validator, RoomUserService roomUserService, ExciseService exciseService,
        JwtService jwtService)
    {
        _exciseService = exciseService;
        _validator = validator;
        _roomUserService = roomUserService;
        _jwtService = jwtService;
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

        if (string.IsNullOrWhiteSpace(message))
        {
            await CloseWithMessageAsync(socket, "Empty auth message", "Missing authentication message");
            throw new InvalidOperationException("Received empty WebSocket auth message");
        }

        JsonDocument json;
        try
        {
            json = JsonDocument.Parse(message);
        }
        catch (JsonException ex)
        {
            await CloseWithMessageAsync(socket, "Invalid JSON", "Invalid authentication message format");
            Console.WriteLine($"❌ Invalid JSON: {ex.Message}");
            throw;
        }

        var root = json.RootElement;

        if (!root.TryGetProperty("type", out var typeProp) ||
            !root.TryGetProperty("token", out var tokenProp))
        {
            await CloseWithMessageAsync(socket, "Missing fields", "Missing type or token in auth message");
            throw new InvalidOperationException("Missing type/token fields in auth JSON");
        }

        var type = typeProp.GetString();
        if (type != "auth")
        {
            await CloseWithMessageAsync(socket, "Invalid type", "Expected type='auth'");
            throw new InvalidOperationException($"Invalid auth type: {type}");
        }

        var token = tokenProp.GetString();
        if (string.IsNullOrWhiteSpace(token))
        {
            await CloseWithMessageAsync(socket, "Missing token", "Token cannot be empty");
            throw new InvalidOperationException("Token is missing or empty");
        }

        var clientId = _jwtService.GetClientIdFromToken(token);
        if (string.IsNullOrEmpty(clientId))
        {
            await CloseWithMessageAsync(socket, "Unauthorized", "Invalid or expired token");
            throw new UnauthorizedAccessException("Invalid JWT token");
        }

        Console.WriteLine($"✅ Authenticated client {clientId}");
        return Guid.Parse(clientId);
    }

    private static async Task CloseWithMessageAsync(WebSocket socket, string message, string reason)
    {
        var msg = Encoding.UTF8.GetBytes(message);
        await socket.SendAsync(new ArraySegment<byte>(msg), WebSocketMessageType.Text, true, CancellationToken.None);
        await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, reason, CancellationToken.None);
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