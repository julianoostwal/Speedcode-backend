using speedCode.dto.room;

namespace speedCode.rooms;

public static class PlayerManager
{
    public static void AddPlayer(string roomId, Player player) => WSRoomsService.JoinRoom(roomId, player);
    public static void RemovePlayer(string roomId, string clientId) => WSRoomsService.LeaveRoom(roomId, clientId);

    public static async Task markPlayerCompleted(string roomId, string clientId)
    {
        if (WSRoomsService.Rooms.TryGetValue(roomId, out var room))
        {
            room.Players[clientId].IsCompleted = true;
        }

        await room.BroadcastAsync(new UpdateRoomStateDto
        {
            Players = room.Players.Values.Select(p => new PlayerStateDto
            {
                ClientId = p.ClientId,
                IsCompleted = p.IsCompleted
            }).ToList()
        });
    }

    public static void AfterCodeTest()
    {
    }
}