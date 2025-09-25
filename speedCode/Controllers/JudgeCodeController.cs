using Microsoft.AspNetCore.Mvc;

namespace speedCode.Controllers;

/// <summary>
/// Controller die doet het gedeelte van de code testen of hij op het verwachting is
/// </summary>
[ApiController]
[Route("[controller]")]
public class JudgeCodeController : ControllerBase
{
    [HttpGet("judgecode")]
    public IActionResult Get(string code, string languageId, string exerciseId)
    {
        return Ok("Hello World!");
    }
}