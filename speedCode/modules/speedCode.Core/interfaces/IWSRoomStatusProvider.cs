namespace speedCode.Core.interfaces;

public interface IWSRoomStatusProvider
{
    (bool isStarted, int activePlayers) GetRoomStatus(Guid roomId);
}
