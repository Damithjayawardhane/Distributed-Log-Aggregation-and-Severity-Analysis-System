using DCAssigmentWebApplication.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplicationDC.Models;

namespace WebApplicationDC.Controllers
{
    [ApiController]
    [Route("mapper")]
    public class MapperController : ControllerBase
    {
        [HttpPost("process")]
        public IActionResult Process([FromBody] LogChunk chunk)
        {
            var result = MapperService.Process(chunk);
            return Ok(result);
        }
    }
}
