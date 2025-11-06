using speedCode.Core.interfaces;

namespace speedCode.rooms.service;

public class WSRoomStatusProvider : IWSRoomStatusProvider
{
    public (bool isStarted, int activePlayers) GetRoomStatus(Guid roomId)
    {
        if (WSRoomsService.Rooms.TryGetValue(roomId, out var room))
        {
            return (room.IsStarted, room.Players.Count);
        }
        
        return (false, 0);
    }
}
