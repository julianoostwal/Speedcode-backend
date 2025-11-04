using System.Net.WebSockets;
using System.Text.Json;
using speedCode.rooms;
using speedCode.rooms.service;
using speedCode.WsRoom.interfaces;

namespace speedCode.WsRoom.handlers;

public class StartRoomHandler : IWebSocketActionHandler
{
    private readonly WSService _wsService;

    public string Action => "start";

    public StartRoomHandler(WSService wsService)
    {
        _wsService = wsService;
    }

    public async Task HandleAsync(WebSocket socket, JsonElement payload, Guid roomId)
    {
        var clientId = await _wsService.ValidateAndExtractClientIdAsync(socket, payload);
        WSRoomsService.StartRoom(roomId, clientId);
    }
}