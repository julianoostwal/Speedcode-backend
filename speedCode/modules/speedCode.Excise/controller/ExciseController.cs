using Microsoft.AspNetCore.Mvc;
using speedCode.Core.judge0.dto;
using speedCode.Excise.model;
using speedCode.services;
using speedCode.Infrastructure;
using speedCode.Excise.dtos;

namespace speedCode.Excise.controller;

[ApiController]
[Route("[controller]")]
public class ExciseController : ControllerBase
{
    private readonly ExciseService _exciseService;
    private readonly WebHelper _webHelper;

    public ExciseController(ExciseService exciseService, WebHelper webHelper)
    {
        _exciseService = exciseService;
        _webHelper = webHelper;
    }

    /// <summary>
    /// Get all exercises
    /// </summary>
    //! moet nog verder uitgewerkt worden
    /*
    [HttpGet]
    public async Task<ActionResult<List<ExerciseFullDto>>> GetAllExercises()
    {
        var exercises = _exciseService.GetAllExercises();
        return Ok(exercises);
    }
    */

    [HttpPost("judgecode")]
    public async Task<TestCodeAsyncResults> Post([FromBody] TestCodeRequest request)
    {
        return await _exciseService.TestCodeAsync(request);
    }
}