using speedCode.WsRoom;

namespace speedCode.rooms;

using System.Net.WebSockets;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

public class Room
{
    public Guid RoomId { get; set; }

    public required Guid CreatedBy { get; set; }
    public bool IsStarted { get; set; } = false;

    public ConcurrentDictionary<Guid, Player> Players { get; set; } = new();


    public void AddPlayer(Player player)
    {
        Players[player.Profile.Id] = player;
    }

    public async Task BroadcastRoomUpdateAsync()
    {
        var update = new UpdateRoomStateDto
        {
            Players = Players.Values.Select(p => new PlayerStateDto
            {
                Profiles = p.Profile,
                Finishedexercise = p.Finishedexercise
            }).ToList()
        };

        string json = JsonSerializer.Serialize(update);
        var buffer = Encoding.UTF8.GetBytes(json);
        var segment = new ArraySegment<byte>(buffer);

        foreach (var player in Players.Values)
        {
            if (player.Socket.State == WebSocketState.Open)
            {
                await player.Socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

    public void RemovePlayer(Guid clientId)
    {
        Players.TryRemove(clientId, out _);
    }
}