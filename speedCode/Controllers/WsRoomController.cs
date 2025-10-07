using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using speedCode.dto;
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

    [HttpGet("testCode/{roomId}/{clientId}")]
    public async Task<TestCodeAsyncResults> Post(string roomId, string clientId, [FromBody] TestCodeRequest request)
    {
        
        return await _judgeCodeService.TestCodeAsync(request);
    }
}