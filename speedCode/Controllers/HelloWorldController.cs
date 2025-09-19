using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace speedCode.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloWorldController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok("Hello World!");
        }
    }
}