using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using speedCode.codeTesting;

namespace speedCode.Controllers
{
    /// <summary>
    /// first controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HelloWorldController : ControllerBase
    {
        private readonly Judge0Client _judge0Client;

        public HelloWorldController(Judge0Client judge0Client)
        {
            _judge0Client = judge0Client;
        }


        /// <summary>
        /// Return hello world
        /// </summary>
        /// <remarks>Must be authenticated to use this API</remarks>
        /// <response code="200">Return hello world</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Get()
        {
            return Ok("Hello World!");
        }

        [HttpGet("submit-code")]
        public async Task<IActionResult> SubmitCode()
        {
            var result = await _judge0Client.SubmitCodeAsync("print('Hello World')", 71);
    
            return Ok(new
            {
                Output = result.Stdout,
                Error = result.Stderr,
                Status = result.Status?.Description
            });
        }

    }
}