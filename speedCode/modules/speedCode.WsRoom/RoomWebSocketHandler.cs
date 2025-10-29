using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using speedCode.Core.judge0.dto;
using speedCode.Core.utils;
using speedCode.Excise.model;
using speedCode.Excise.service;
using speedCode.Profile;
using speedCode.rooms;
using speedCode.services;
using speedCode.WsRoom.DTO;

namespace speedCode.Excise;

public class RoomWebSocketHandler
{
    private readonly RoomValidator _validator;
    private readonly RoomUserService _roomUserService;
    private readonly JwtService _jwtService;
    private readonly RoomService _roomService;
    private readonly ProfileDb _profileDb;
    private readonly ExciseDb _dbContext;
    private readonly ExciseService _exciseService;

    public RoomWebSocketHandler(RoomValidator validator, RoomUserService roomUserService, ExciseService exciseService,
        ProfileDb profileDb,
        RoomService roomService,
        JwtService jwtService)
    {
        _profileDb = profileDb;
        _roomService = roomService;
        _exciseService = exciseService;
        _validator = validator;
        _roomUserService = roomUserService;
        _jwtService = jwtService;
    }

    public async Task HandleAsync(Guid roomId, WebSocket socket)
    {
        Guid clientId = await AuthenticateAsync(socket);
        if (!await _validator.RoomExistsAsync(roomId) || !await _validator.ClientExistsAsync(clientId) ||
            !await _validator.ClientAllowToJoin(roomId, clientId))
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
            FinishedExcise = _roomService.GetAllFinishedExcisesByRoomAndUserId(roomId, clientId)
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

    public async Task<TestCodeAsyncResults> TestCodeAsyncForWSRoom(TestCodeRequest request, Guid roomId,
        Guid clientId)
    {
        TestCodeAsyncResults result = await _exciseService.TestCodeAsync(request);

        if (result.IsPassed)
        {
            Double averageMemory = result.TestProcess
                .Where(x => x.Logs != null && x.Logs.Any())
                .SelectMany(x => x.Logs)
                .Select(log => log.result.Memory)
                .ToList().Average();


            Double averageTime = result.TestProcess
                .Where(x => x.Logs != null && x.Logs.Any())
                .SelectMany(x => x.Logs)
                .Select(log => log.result.Time)
                .Select(t => double.Parse(t))
                .Average();


            CompletedProfileExercise completedProfileExercise = new CompletedProfileExercise()
            {
                profileId = clientId,
                exerciseId = request.ExerciseId,
                memory = averageMemory,
                time = TimeSpan.FromMilliseconds(averageTime),
            };
            _profileDb.CompletedProfileExercise.Add(completedProfileExercise);

            if (WSRoomsService.Rooms.TryGetValue(roomId, out var roomValue))
            {
                PlayerManager.CompletedAExercise(new FinishARoomExerciseDto()
                {
                    clientId = clientId,
                    ExerciseId = request.ExerciseId,
                    Room = roomValue,
                    memory = averageMemory,
                    time = TimeSpan.FromMilliseconds(averageTime),
                });
            }

            await _dbContext.SaveChangesAsync();
        }

        return result;
    }
}