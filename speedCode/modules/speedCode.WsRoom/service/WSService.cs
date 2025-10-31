using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using speedCode.Core.interfaces;

namespace speedCode.rooms.service;

public class WSService
{
    private IJwtService _jwtService;

    public WSService(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public static async Task CloseWithMessageAsync(WebSocket socket, string message, string reason)
    {
        var msg = Encoding.UTF8.GetBytes(message);
        await socket.SendAsync(new ArraySegment<byte>(msg), WebSocketMessageType.Text, true, CancellationToken.None);
        await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, reason, CancellationToken.None);
    }

    public async Task<JsonElement> ReceiveJsonAsync(WebSocket socket)
    {
        var buffer = new byte[1024 * 4];
        var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

        if (string.IsNullOrWhiteSpace(message))
        {
            await CloseWithMessageAsync(socket, "Empty message", "No message received");
            throw new InvalidOperationException("Received empty WebSocket message");
        }

        try
        {
            return JsonDocument.Parse(message).RootElement;
        }
        catch (JsonException ex)
        {
            await CloseWithMessageAsync(socket, "Invalid JSON", ex.Message);
            throw;
        }
    }

    public string ValidateAndReturnJsonProperty(string property, JsonElement json)
    {
        if (!json.TryGetProperty(property, out var prop))
        {
            throw new InvalidOperationException($"Missing '{property}' field");
        }

        var value = prop.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Empty '{property}' field");
        }

        return value;
    }

    public async Task<Guid> ValidateAndExtractClientIdAsync(WebSocket socket, JsonElement json)
    {
        string token = ValidateAndReturnJsonProperty("token", json);
        var clientId = _jwtService.GetClientIdFromToken(token);
        if (string.IsNullOrEmpty(clientId))
        {
            await CloseWithMessageAsync(socket, "Unauthorized", "Invalid or expired token");
            throw new UnauthorizedAccessException("Invalid JWT token");
        }

        return Guid.Parse(clientId);
    }
}