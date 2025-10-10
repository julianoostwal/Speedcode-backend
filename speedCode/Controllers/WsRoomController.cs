using Microsoft.AspNetCore.Mvc;
using speedCode.Data;
using speedCode.dto;
using speedCode.dto.room;
using speedCode.rooms;
using speedCode.services;

namespace speedCode.Controllers;

[ApiController]
[Route("ws")]
public class WsRoomController : ControllerBase
{
    private readonly JudgeCodeService _judgeCodeService;

    public WsRoomController(JudgeCodeService judgeCodeService)
    {
        _judgeCodeService = judgeCodeService;
    }

    /// <summary>
    /// Connect to a room via WebSocket.
    /// <para>After connecting, the client must send an authentication token in the format:</para>
    /// <code>{ "type": "auth", "token": "YOUR_JWT" }</code>
    /// </summary>
    /// <param name="roomId">The ID of an existing room in the database.</param>
    [HttpGet("{roomId}")]
    public async Task Get(string roomId, [FromServices] AppDbContext db)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsync("WebSocket request expected.");
            return;
        }

        var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        var validator = new RoomValidator(db);
        var handler = new WebSocketHandler(validator);
        await handler.HandleAsync(roomId, socket);
    }

    /// <summary>
    /// Test a player's code submission within a room.
    /// </summary>
    /// <remarks>
    /// This endpoint evaluates the code submitted by a player in a specific room.
    /// After evaluation, it broadcasts the updated room state to all participants.
    /// </remarks>
    /// <param name="roomId">The ID of the room where the code is being tested.</param>
    /// <param name="clientId">The ID of the client (user) submitting the code, logged in via Supabase.</param>
    /// <param name="request">The code submission request containing the code and language information.</param>
    /// <returns>Returns the results of the code test, including pass/fail status and logs.</returns>
    /// <response code="200">Code tested successfully, returns the test results.</response>
    /// <response code="404">Room or client not found.</response>
    /// <response code="400">Invalid or missing request data.</response>
    [HttpPost("testCode/{roomId}/{clientId}")]
    public async Task<ActionResult<TestCodeAsyncResults>> TestCodeAsync(
        string roomId, string clientId, [FromBody] TestCodeRequest request)
    {
        if (!WSRoomsService.Rooms.TryGetValue(roomId, out var room))
            return NotFound($"Room {roomId} not found.");

        if (!room.Players.TryGetValue(clientId, out var player))
            return NotFound($"Client {clientId} not found.");
        
        var result = await _judgeCodeService.TestCodeAsyncForRoom(request,roomId, clientId);
        return Ok(result);
    }
}