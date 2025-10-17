using System.Collections.Concurrent;


namespace speedCode.rooms;

public static class WSRoomsService
{
    public static ConcurrentDictionary<Guid, Room> Rooms { get; private set; } = new();

    public static Room GetOrCreateRoom(Guid roomId)
    {
        return Rooms.GetOrAdd(roomId, id => new Room { RoomId = id });
    }

    public static void JoinRoom(Room room, Player player)
    {
        GetOrCreateRoom(room.RoomId).AddPlayer(player);
    }

    public static void LeaveRoom(Room room, Guid clientId)
    {
        room.RemovePlayer(clientId);
        if (room.Players.IsEmpty)
            Rooms.TryRemove(room.RoomId, out _);
    }
}