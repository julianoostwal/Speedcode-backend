using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using speedCode.Core.interfaces;
using speedCode.rooms;
using speedCode.rooms.service;
using speedCode.WsRoom.interfaces;

namespace speedCode.WsRoom.handlers;

public class JoinHandler : IWebSocketActionHandler
{
    public string Action => "join";

    private readonly WSService _wsService;
    private readonly IRoomDataService _roomDataService;
    private readonly IProfileQueryService _profileQueryService;

    public JoinHandler(WSService wsService, IRoomDataService roomDataService, IProfileQueryService profileQueryService)
    {
        _profileQueryService = profileQueryService;
        _wsService = wsService;
        _roomDataService = roomDataService;
    }

    public async Task HandleAsync(WebSocket socket, JsonElement payload, Guid roomId)
    {
        var clientId = await _wsService.ValidateAndExtractClientIdAsync(socket, payload);

        // Get existing room or create new one if it doesn't exist
        var room = WSRoomsService.GetOrCreateRoom(roomId, clientId);

        var profileDto = await _profileQueryService.GetByIdAsync(clientId);
        
        var player = new Player
        {
            Profile = profileDto!,
            Socket = socket,
            ConnectedAt = DateTime.UtcNow,
            Finishedexercise = await _roomDataService.GetFinishedExercisesByRoomAndUserAsync(roomId, clientId)
        };
        PlayerManager.AddPlayer(room, player);
    }
}