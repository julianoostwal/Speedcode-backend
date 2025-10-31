using speedCode.WsRoom.DTO;

namespace speedCode.rooms;

public static class PlayerManager
{
    public static void AddPlayer(Room room, Player player) => WSRoomsService.JoinRoom(room, player);
    public static void RemovePlayer(Room room, Guid clientId) => WSRoomsService.LeaveRoom(room, clientId);

    public static void CompletedAExercise(FinishARoomExerciseDto dto)
    {
        dto.Room.Players[dto.clientId].FinishedExcise.Add(dto.ExerciseId);
        dto.Room.BroadcastRoomUpdateAsync();
    }
}