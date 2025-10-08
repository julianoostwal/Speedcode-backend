using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using speedCode.dto;
using speedCode.poco;
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

    [HttpGet("{roomId}/{clientId}")]
    public async Task Get(string roomId, string clientId)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await WSRoomsService.HandleClientAsync(roomId, clientId, socket);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }

    [HttpPost("testCode/{roomId}/{clientId}")]
    public async Task<ApiResult<TestCodeAsyncResults>> TestCodeAsync(
        string roomId, string clientId, [FromBody] TestCodeRequest request)
    {
        if (!WSRoomsService.Rooms.TryGetValue(roomId, out var room))
            return ApiResult<TestCodeAsyncResults>.Fail($"Room {roomId} not found.");

        if (!room.Players.ContainsKey(clientId))
            return ApiResult<TestCodeAsyncResults>.Fail($"Client {clientId} not found.");

        var result = await _judgeCodeService.TestCodeAsync(request);
        return ApiResult<TestCodeAsyncResults>.Ok(result);
    }

}