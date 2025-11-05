using Microsoft.AspNetCore.Mvc;
using speedCode.Core.judge0.dto;
using speedCode.Exercise.service;
using speedCode.Infrastructure;

namespace speedCode.Exercise.controller;

[ApiController]
[Route("[controller]")]
public class ExerciseController : ControllerBase
{
    private readonly ExerciseService _exerciseService;
    private readonly WebHelper _webHelper;

    public ExerciseController(ExerciseService exerciseService, WebHelper webHelper)
    {
        _exerciseService = exerciseService;
        _webHelper = webHelper;
    }

    [HttpPost("judgecode")]
    public async Task<TestCodeAsyncResults> Post([FromBody] TestCodeRequest request)
    {
        return await _exerciseService.TestCodeAsync(request);
    }
}