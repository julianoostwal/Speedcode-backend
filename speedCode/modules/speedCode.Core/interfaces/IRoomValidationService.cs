namespace speedCode.Core.interfaces;

public interface IRoomValidationService
{
    Task<bool> RoomExistsAsync(Guid roomId);
    Task<bool> ClientExistsAsync(Guid clientId);
    Task<bool> ClientAllowToJoinAsync(Guid roomId, Guid clientId);
    Task<bool> IsRoomAdminAsync(Guid roomId, Guid clientId);
}