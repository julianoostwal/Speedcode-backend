using Microsoft.AspNetCore.Mvc;
using speedCode.Core.judge0.dto;
using speedCode.exercise.model;
using speedCode.services;
using speedCode.Infrastructure;

namespace speedCode.exercise.controller;

[ApiController]
[Route("[controller]")]
public class exerciseController : ControllerBase
{
    private readonly exerciseService _exerciseService;
    private readonly WebHelper _webHelper;

    public exerciseController(exerciseService exerciseService, WebHelper webHelper)
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