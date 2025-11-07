using speedCode.Core.interfaces;
using speedCode.rooms;

namespace speedCode.WsRoom;

public class RoomValidator
{
    private readonly IRoomValidationService _validationService;
    private readonly IProfileQueryService _profileQueryService;

    public RoomValidator(IRoomValidationService validationService, IProfileQueryService profileQueryService)
    {
        _validationService = validationService;
        _profileQueryService = profileQueryService;
    }

    public async Task<bool> RoomExistsAsync(Guid roomId)
    {
        return await _validationService.RoomExistsAsync(roomId);
    }

    public async Task<bool> ClientExistsAsync(Guid clientId)
    {
        var profile = await _profileQueryService.GetByIdAsync(clientId);
        return profile != null;
    }

    public bool IsRoomStarted(Guid roomId)
    {
        try
        {
            WSRoomsService.GetRoom(roomId);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<bool> ClientAllowToJoin(Guid roomId, Guid clientId)
    {
        return await _validationService.ClientAllowToJoinAsync(roomId, clientId);
    }

    public async Task<bool> isRoomAdmin(Guid roomId, Guid clientId)
    {
        return await _validationService.IsRoomAdminAsync(roomId, clientId);
    }
}