using Microsoft.AspNetCore.Mvc;
using speedCode.Core.rooms.dto;
using speedCode.Excise.model;
using speedCode.Room.dtos;
using speedCode.Room.Model;
using speedCode.Room.service;

namespace speedCode.Room.controller;

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
    /// get all rooms with exercises and profiles
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAllRooms()
    {
        var rooms = await _RoomService.GetAllRooms();
        return Ok(rooms);
    }

    /// <summary>
    /// get room with exercises and profiles by id
    /// </summary>
    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetRoom(Guid id)
    {
        var room = await _RoomService.GetRoomById(id);
        if (room == null)
        {
            return NotFound();
        }

        return Ok(room);
    }

    [HttpPut("/updateRoomExercise")]
    public IActionResult UpdateRoomExercise(updateRoomExercisesDTO dto)
    {
        return Ok(_RoomService.UpdateRoomExercises(dto));
    }

    [HttpPut("/updateRoom")]
    public IActionResult UpdateRoomDTO(UpdateRoomDTO dto)
    {
        return Ok(_RoomService.UpdateRoom(dto));
    }

    [HttpPut("/updateRoomUsers")]
    public IActionResult UpdateRoomUsers(updateRoomUsersDTO dto)
    {
        return Ok(_RoomService.UpdateRoomUsers(dto));
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