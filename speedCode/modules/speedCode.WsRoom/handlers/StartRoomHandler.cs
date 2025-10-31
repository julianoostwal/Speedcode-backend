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

    public async Task HandleAsync(WebSocket socket, JsonElement payload)
    {
        var json = await _wsService.ReceiveJsonAsync(socket);
        var clientId = await _wsService.ValidateAndExtractClientIdAsync(socket, json);
        Guid roomId = Guid.Parse(_wsService.ValidateAndReturnJsonProperty("roomId", payload));
        WSRoomsService.StartRoom(roomId, clientId);
    }
}