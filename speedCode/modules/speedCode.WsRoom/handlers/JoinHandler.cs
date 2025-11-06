using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using speedCode.Profile.service;
using speedCode.Room.service;
using speedCode.rooms;
using speedCode.rooms.service;
using speedCode.WsRoom.interfaces;

namespace speedCode.WsRoom.handlers;

public class JoinHandler : IWebSocketActionHandler
{
    public string Action => "join";

    private readonly WSService _wsService;
    private readonly RoomService _roomService;
    private readonly ProfileService _profileService;

    public JoinHandler(WSService wsService, RoomService roomService, ProfileService profileService)
    {
        _profileService = profileService;
        _wsService = wsService;
        _roomService = roomService;
    }


    public async Task HandleAsync(WebSocket socket, JsonElement payload, Guid roomId)
    {
        var clientId = await _wsService.ValidateAndExtractClientIdAsync(socket, payload);

        rooms.Room room = WSRoomsService.GetRoom(roomId);
        var player = new Player
        {
            Profile = _profileService.GetProfileById(clientId),
            Socket = socket,
            ConnectedAt = DateTime.UtcNow,
            Finishedexercise = _roomService.GetAllFinishedexercisesByRoomAndUserId(roomId, clientId)
        };
        PlayerManager.AddPlayer(room, player);
    }
}