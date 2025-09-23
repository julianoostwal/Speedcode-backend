using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace speedCode.Controllers
{
    /// <summary>
    /// first controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HelloWorldController : ControllerBase
    {
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
    }
}