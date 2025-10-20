namespace speedCode.rooms;

public static class PlayerManager
{
    public static void AddPlayer(Room room, Player player) => WSRoomsService.JoinRoom(room, player);
    public static void RemovePlayer(Room room, Guid clientId) => WSRoomsService.LeaveRoom(room, clientId);

    public static void CompletedAExercise(Room room, Guid clientId, Guid finishedExcise)
    {
        room.Players[clientId].FinishedExcise.Add(finishedExcise);
        room.BroadcastRoomUpdateAsync();
    }
}