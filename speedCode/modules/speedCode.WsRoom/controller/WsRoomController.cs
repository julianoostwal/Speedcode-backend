using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using speedCode.Core.judge0.dto;
using speedCode.Excise;
using speedCode.rooms;
using speedCode.services;

namespace speedCode.WsRoom.controller;

[ApiController]
[Route("ws")]
public class WsRoomController : ControllerBase
{
    private readonly ExciseService _exciseService;
    private readonly RoomWebSocketHandler _wsHandler;

    public WsRoomController(ExciseService exciseService, RoomWebSocketHandler wsHandler)
    {
        _wsHandler = wsHandler;
    }

    /// <summary>
    /// Connect to a room via WebSocket.
    /// <para>After connecting, the client must send an authentication token in the format:</para>
    /// <code>{ "type": "auth", "token": "YOUR_JWT" }</code>
    /// </summary>
    /// <param name="roomId">The ID of an existing room in the database.</param>
    [HttpGet("{roomId}")]
    public async Task Get(Guid roomId, [FromServices] ExciseDb db)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsync("WebSocket request expected.");
            return;
        }

        var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        await _wsHandler.HandleAsync(roomId, socket);
    }

    /// <summary>
    /// Test a player's code submission within a room.
    /// </summary>
    /// <remarks>
    /// This endpoint evaluates the code submitted by a player in a specific room.
    /// After evaluation, it broadcasts the updated room state to all participants.
    /// </remarks>
    /// <param name="roomId">The ID of the room where the code is being tested.</param>
    /// <param name="request">The code submission request containing the code and language information.</param>
    /// <returns>Returns the results of the code test, including pass/fail status and logs.</returns>
    /// <response code="200">Code tested successfully, returns the test results.</response>
    /// <response code="404">Room or client not found.</response>
    /// <response code="400">Invalid or missing request data.</response>
    [HttpPost("{roomId:guid}/test")]
    public async Task<ActionResult<TestCodeAsyncResults>> TestCodeAsync(
        Guid roomId, [FromBody] TestCodeRequest request)
    {
        if (!WSRoomsService.Rooms.TryGetValue(roomId, out var room))
            return NotFound($"Room {roomId} not found.");
        var clientIdStr = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(clientIdStr) || !Guid.TryParse(clientIdStr, out var clientId))
            return Unauthorized();

        if (!room.Players.TryGetValue(clientId, out var player))
            return NotFound($"Client {clientId} not found in room.");

        try
        {
            var result = await _wsHandler.TestCodeAsyncForWSRoom(request, roomId, clientId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal error while testing code: {ex.Message}");
        }
    }
}