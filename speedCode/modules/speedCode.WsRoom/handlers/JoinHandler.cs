using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
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

    public JoinHandler(WSService wsService, RoomService roomService)
    {
        _wsService = wsService;
        _roomService = roomService;
    }


    public async Task HandleAsync(WebSocket socket, JsonElement payload, Guid roomId)
    {
        var clientId = await _wsService.ValidateAndExtractClientIdAsync(socket, payload);

        rooms.Room room = WSRoomsService.GetRoom(roomId);
        var player = new Player
        {
            ClientId = clientId,
            Socket = socket,
            ConnectedAt = DateTime.UtcNow,
            FinishedExcise = _roomService.GetAllFinishedExcisesByRoomAndUserId(roomId, clientId)
        };
        PlayerManager.AddPlayer(room, player);
        await ReceiveLoopAsync(room, player);
    }

    private async Task ReceiveLoopAsync(rooms.Room room, Player player)
    {
        var buffer = new byte[1024 * 4];
        await room.BroadcastRoomUpdateAsync();

        try
        {
            while (player.Socket.State == WebSocketState.Open)
            {
                var result = await player.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    PlayerManager.RemovePlayer(room, player.ClientId);
                    await player.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the player",
                        CancellationToken.None);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            PlayerManager.RemovePlayer(room, player.ClientId);
        }
    }
}