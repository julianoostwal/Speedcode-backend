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

    public void RemovePlayer(string clientId)
    {
        Players.TryRemove(clientId, out _);
    }
}