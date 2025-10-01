using Microsoft.AspNetCore.Mvc;
using speedCode.codeTesting;
using speedCode.services;

namespace speedCode.Controllers;

/// <summary>
/// Controller die doet het gedeelte van de code testen of hij op het verwachting is
/// </summary>
[ApiController]
[Route("[controller]")]
public class JudgeCodeController : ControllerBase
{
    private readonly JudgeCodeService _judgeCodeService;

    public JudgeCodeController(JudgeCodeService judgeCodeService)
    {
        _judgeCodeService = judgeCodeService;
    }

    [HttpGet("judgecode")]
    public IActionResult Get()
    {
        // _judgeCodeService.TestCodeAsync(code, Guid.Parse(exerciseId), 70);
        _judgeCodeService.TestCodeAsync();

        return Ok("Hello World!");
    }
}