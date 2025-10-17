using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using speedCode.Core.rooms.dto;
using speedCode.Excise.model;
using speedCode.Excise.service;
using speedCode.Room.Model;
using speedCode.services;

namespace speedCode.Excise.controller;

[ApiController]
[Route("roomExcise")]
public class RoomExciseController : ControllerBase
{
    private readonly RoomService _RoomService;
    private readonly ExciseService _exciseService;

    public RoomExciseController(RoomService roomService, ExciseService exciseService)
    {
        _RoomService = roomService;
        _exciseService = exciseService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllRooms()
    {
        var rooms = await _exciseService.GetAllRooms();
        return Ok(rooms);
    }

    //! nog mee bezig
    /*[HttpGet("id/{id}")]
    public async Task<IActionResult> GetRoom(Guid id)
    {
        var room = await _RoomService.GetRoomById(id);
        if (room == null)
        {
            return NotFound();
        }
        return Ok(room);
    }*/
}