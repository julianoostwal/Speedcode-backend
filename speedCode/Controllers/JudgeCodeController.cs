using Microsoft.AspNetCore.Mvc;
using speedCode.codeTesting;
using speedCode.dto;
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

    [HttpPost("judgecode")]
    public async Task<TestCodeAsyncResults> Post([FromBody] TestCodeRequest request)
    {
        return await _judgeCodeService.TestCodeAsync(request);
    }
}