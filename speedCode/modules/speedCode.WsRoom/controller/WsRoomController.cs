using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using speedCode.Core.attribute;
using speedCode.Core.interfaces;
using speedCode.Core.judge0.dto;
using speedCode.rooms;
using speedCode.services;
using speedCode.WsRoom.handlers;

namespace speedCode.WsRoom.controller;

[ApiController]
[Route("ws")]
public class WsRoomController : ControllerBase
{
    private readonly RoomWebSocketHandler _wsHandler;
    private readonly IJwtService _jwtService;

    public WsRoomController(exerciseService exerciseService, RoomWebSocketHandler wsHandler, IJwtService jwtService)
    {
        _jwtService = jwtService;
        _wsHandler = wsHandler;
    }

    /// <summary>
    /// Connect to a room via WebSocket.
    /// <para>there is 3 types to using. onli the host can start and create the WS rooms</para>
    /// <code>{ "type": "start", "token": "YOUR_JWT","roomId":"" }</code>
    /// /// <code>{ "type": "create", "token": "YOUR_JWT","roomId":"" }</code>
    /// /// <code>{ "type": "join", "token": "YOUR_JWT","roomId":"" }</code>
    /// </summary>
    /// <param name="roomId">The ID of an existing room in the database.</param>
    [HttpGet]
    public async Task Get()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsync("WebSocket request expected.");
            return;
        }

        var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        await _wsHandler.HandleAsync(socket);
    }


    /// <summary>
    /// Get all rooms where is online
    /// </summary>
    /// <returns>Returns a list of rooms where the user is connected</returns>
    /// <response code="200">Returns a list of rooms where the user is connected</response>
    /// <response code="401">Missing or invalid Authorization header</response>
    /// <response code="403">Invalid or expired token</response>
    [HttpGet("online")]
    public ActionResult GetOnlineRooms()
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return Unauthorized("Missing or invalid Authorization header");
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        var clientId = _jwtService.GetClientIdFromToken(token);
        if (string.IsNullOrEmpty(clientId))
        {
            return Unauthorized("Invalid or expired token");
        }

        return Ok(WSRoomsService.GetOnlineRoomsById(Guid.Parse(clientId)));
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
    [HttpPost("/test/{roomId:guid}")]
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