using DCAssigmentWebApplication.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplicationDC.Models;

namespace WebApplicationDC.Controllers
{
    [ApiController]
    [Route("validator")]
    public class ValidatorController : ControllerBase
    {
        [HttpPost("validate")]
        public IActionResult Validate([FromBody] ValidationRequest request)
        {
            var result = ValidatorService.Validate(request.Chunk, request.MapperResult);
            return Ok(result);
        }
    }
}