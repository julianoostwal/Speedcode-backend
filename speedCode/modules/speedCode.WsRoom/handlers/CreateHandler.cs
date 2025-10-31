using System.Net.WebSockets;
using System.Text.Json;
using speedCode.Room.Model;
using speedCode.Room.service;
using speedCode.rooms;
using speedCode.rooms.service;
using speedCode.WsRoom.interfaces;

namespace speedCode.WsRoom.handlers;

public class CreateHandler : IWebSocketActionHandler
{
    public string Action => "create";

    private readonly WSService _wsService;
    private readonly RoomService _roomService;

    public CreateHandler(WSService wsService, RoomService roomService)
    {
        _wsService = wsService;
        _roomService = roomService;
    }

    public async Task HandleAsync(WebSocket socket, JsonElement payload)
    {
        var json = await _wsService.ReceiveJsonAsync(socket);
        var clientId = await _wsService.ValidateAndExtractClientIdAsync(socket, json);
        Guid roomId = Guid.Parse(_wsService.ValidateAndReturnJsonProperty("roomId", payload));
        Rooms room = await _roomService.GetRoomById(roomId);

        if (clientId == room.CreatedById)
        {
            WSRoomsService.CreateRoom(roomId, clientId);
        }
        else
        {
            throw new UnauthorizedAccessException("You are not allowed to create a room");
        }
    }
}