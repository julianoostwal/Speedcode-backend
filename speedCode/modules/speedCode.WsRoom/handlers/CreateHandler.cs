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

    public async Task HandleAsync(WebSocket socket, JsonElement payload, Guid roomId)
    {
        var clientId = await _wsService.ValidateAndExtractClientIdAsync(socket, payload);
        Rooms room = await _roomService.GetRoomById(roomId);

        if (clientId == room.CreatedById)
        {
            try
            {
                WSRoomsService.CreateRoom(roomId, clientId);
            }
            catch (Exception e)
            {
                socket.CloseAsync(WebSocketCloseStatus.InternalServerError, e.Message, CancellationToken.None);
                throw;
            }
        }
        else
        {
            socket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "You are not allowed to create a room",
                CancellationToken.None);
            throw new UnauthorizedAccessException("You are not allowed to create a room");
        }
    }
}