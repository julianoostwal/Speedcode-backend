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

    public static void JoinRoom(Room room, Player player)
    {
        GetOrCreateRoom(room.RoomId).AddPlayer(player);
    }

    public static void LeaveRoom(Room room, string clientId)
    {
        room.RemovePlayer(clientId);
        if (room.Players.IsEmpty)
            Rooms.TryRemove(room.RoomId, out _);
    }
}