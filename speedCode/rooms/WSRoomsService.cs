using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using Microsoft.EntityFrameworkCore;
using speedCode.Data;
using speedCode.security;

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
}