namespace speedCode.rooms;

using System.Net.WebSockets;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

public class Room
{
    public string RoomId { get; set; } = "";

    public ConcurrentDictionary<string, Player> Players { get; set; } = new();


    public void AddPlayer(Player player)
    {
        Players[player.ClientId] = player;
    }
    
    public async Task BroadcastAsync(object message)
    {
        string json = JsonSerializer.Serialize(message);
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

    public void RemovePlayer(string clientId)
    {
        Players.TryRemove(clientId, out _);
    }
}