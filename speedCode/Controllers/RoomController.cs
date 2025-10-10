using Microsoft.AspNetCore.Mvc;
using speedCode.dto.room;
using speedCode.Model;
using speedCode.services;

namespace speedCode.Controllers;

[ApiController]
[Route("rooms")]
public class RoomController : ControllerBase
{
    private readonly RoomService _RoomService;

    public RoomController(RoomService roomService)
    {
        _RoomService = roomService;
    }

    /// <summary>
    /// create room
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Rooms>> Post(CreateRoomDTO request)
    {
        Rooms room = await _RoomService.CreateRoom(request);
        return room;
    }
}