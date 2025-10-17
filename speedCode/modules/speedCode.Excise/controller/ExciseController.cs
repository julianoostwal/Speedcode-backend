
using Microsoft.AspNetCore.Mvc;
using speedCode.Core.judge0.dto;
using speedCode.services;

namespace speedCode.Excise;
/// <summary>
/// Controller die doet het gedeelte van de code testen of hij op het verwachting is
/// </summary>
[ApiController]
[Route("[controller]")]
public class ExciseController : ControllerBase
{
    private readonly ExciseService _exciseService;

    public ExciseController(ExciseService exciseService)
    {
        _exciseService = exciseService;
    }

    [HttpPost("judgecode")]
    public async Task<TestCodeAsyncResults> Post([FromBody] TestCodeRequest request)
    {
        return await _exciseService.TestCodeAsync(request);
    }
}