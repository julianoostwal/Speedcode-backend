using System.Collections.Concurrent;


namespace speedCode.rooms;

public static class WSRoomsService
{
    public static ConcurrentDictionary<Guid, Room> Rooms { get; set; } = new();

    public static Room CreateRoom(Guid roomId, Guid profileId)
    {
        if (Rooms.ContainsKey(roomId))
            throw new InvalidOperationException($"Room {roomId} already exists.");

        var room = new Room { RoomId = roomId, CreatedBy = profileId };
        if (!Rooms.TryAdd(roomId, room))
            throw new InvalidOperationException($"Failed to create room {roomId} (possibly a race condition).");

        return room;
    }

    public static List<Room> GetOnlineRoomsById(Guid profileId)
    {
        return Rooms.Values
            .Where(r => r.CreatedBy.Equals(profileId))
            .ToList();
    }

    public static Room StartRoom(Guid roomId, Guid profileId)
    {
        var room = GetRoom(roomId);
        if (room.CreatedBy != profileId)
            throw new InvalidOperationException($"Only the creator :\"{room.CreatedBy} \"can start the room {roomId}.");
        room.IsStarted = true;
        return room;
    }


    public static void JoinRoom(Room room, Player player)
    {
        if (!Rooms.TryGetValue(room.RoomId, out var existingRoom))
            throw new InvalidOperationException($"Room {room.RoomId} does not exist.");

        existingRoom.AddPlayer(player);
    }

    public static Room GetRoom(Guid roomId)
    {
        if (!Rooms.TryGetValue(roomId, out var existingRoom))
            throw new InvalidOperationException($"Room {roomId} does not exist.");

        return existingRoom;
    }


    public static void LeaveRoom(Room room, Guid clientId)
    {
        room.RemovePlayer(clientId);
        if (room.Players.IsEmpty)
            Rooms.TryRemove(room.RoomId, out _);
    }
}