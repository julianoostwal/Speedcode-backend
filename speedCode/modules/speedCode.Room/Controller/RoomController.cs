using Microsoft.AspNetCore.Mvc;
using speedCode.Core.rooms.dto;
using speedCode.Excise.service;
using speedCode.Room.Model;

namespace speedCode.Excise.controller;

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

    
    /// <summary>
    /// generate room with random exercises, for now it is hardcoded to 5 exercises
    /// </summary>
    [HttpPost]
    [Route("/generateroomwithrandomexcises")]
    public ActionResult<Rooms> GenerateRoomWithRandomExercises(CreateRoomDTO request)
    {
        Rooms room = _RoomService.GenerateRoomWithRandomExcises(request, 5);
        return Ok(room);
    }
}