using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace speedCode.rooms;

public static class WSRoomsService
{
    public static ConcurrentDictionary<string, Room> Rooms { get; private set; } = new();

    public static Room GetOrCreateRoom(string roomId)
    {
        return Rooms.GetOrAdd(roomId, id => new Room { RoomId = id });
    }

    public static void JoinRoom(string roomId, Player player)
    {
        var room = GetOrCreateRoom(roomId);
        room.AddPlayer(player);
    }

    public static void LeaveRoom(string roomId, string clientId)
    {
        if (Rooms.TryGetValue(roomId, out var room))
        {
            room.RemovePlayer(clientId);
            if (room.Players.IsEmpty)
                Rooms.TryRemove(roomId, out _);
        }
    }

    public static async Task HandleClientAsync(string roomId, string clientId, WebSocket socket)
    {
        var player = new Player
        {
            ClientId = clientId,
            Socket = socket,
            ConnectedAt = DateTime.UtcNow,
            IsCompleted = false
        };

        JoinRoom(roomId, player);

        var buffer = new byte[1024 * 4];

        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    LeaveRoom(roomId, clientId);
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
                    break;
                }
            }
        }
        catch
        {
            LeaveRoom(roomId, clientId);
        }
    }
}