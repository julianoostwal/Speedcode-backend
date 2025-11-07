using Microsoft.AspNetCore.Mvc;
using speedCode.Core.attribute;
using speedCode.Core.interfaces;
using speedCode.Core.rooms.dto;
using speedCode.Core.utils;
using speedCode.Exercise.model;
using speedCode.Room.dtos;
using speedCode.Room.Model;
using speedCode.Room.service;

namespace speedCode.Room.controller;

[ApiController]
[Route("rooms")]
public class RoomController : ControllerBase
{
    private readonly RoomService _RoomService;
    private readonly IJwtService _jwtService;

    public RoomController(RoomService roomService, IJwtService jwtService)
    {
        _jwtService = jwtService;
        _RoomService = roomService;
    }


    /// <summary>
    /// get all rooms with exercises and profiles
    /// </summary>
    [HttpGet]
    [InjectUserId]
    public async Task<ActionResult<List<RoomFullDto>>> GetAllRooms()
    {
        var userIdStr = User.FindFirst("sub")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();
    
        var rooms = await _RoomService.GetAllRooms(userId);
        return Ok(rooms);
    }

    /// <summary>
    /// get room with exercises and profiles by id
    /// </summary>
    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetRoom(Guid id)
    {
        var room = await _RoomService.GetRoomAndKoppelById(id);
        if (room == null)
        {
            return NotFound();
        }

        return Ok(room);
    }

    [HttpPut("id/{id}")]
    public async Task<IActionResult> UpdateRoomDTO(Guid id, [FromBody] UpdateRoomDTO dto)
    {
        return Ok(await _RoomService.UpdateRoom(id, dto));
    }

    /// <summary>
    /// create room
    /// </summary>
    [HttpPost]
    [InjectUserId]
    public async Task<ActionResult<Rooms>> Post([FromBody] CreateRoomDTO request)
    {
        var room = await _RoomService.CreateRoom(request);
        return Ok(room);
    }

    /// <summary>
    /// generate room with random exercises, for now it is hardcoded to 5 exercises
    /// </summary>
    [HttpPost]
    [Route("/generateroomwithrandomexercises")]
    [InjectUserId]
    public ActionResult<Rooms> GenerateRoomWithRandomExercises(CreateRoomDTO request)
    {
        Rooms room = _RoomService.GenerateRoomWithRandomexercises(request, 5);
        return Ok(room);
    }

    /// <summary>
    /// delete room by id
    /// </summary>
    [HttpDelete("id/{id}")]
    public async Task<IActionResult> DeleteRoom(Guid id)
    {
        var result = await _RoomService.DeleteRoom(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    //! moet nog gemaakt worden Joshua is er mee bezig
    /*
    /// <summary>
    /// Get public leaderboard
    /// </summary>
    [HttpGet("GetLeaderboardPublic")]
    public async Task<ActionResult<List<LeaderboardDto>>> GetLeaderboardPublic()
    {
        var leaderboard = await _RoomService.GetLeaderboardPublic();
        return Ok(leaderboard);
    }*/
}