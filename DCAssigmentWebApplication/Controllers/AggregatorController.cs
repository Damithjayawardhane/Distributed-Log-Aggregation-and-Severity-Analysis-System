using DCAssigmentWebApplication.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplicationDC.Models;

namespace WebApplicationDC.Controllers
{
    [ApiController]
    [Route("aggregator")]
    public class AggregatorController : ControllerBase
    {
        [HttpPost("aggregate")]
        public IActionResult Aggregate([FromBody] SeverityResult result)
        {
            var output = AggregatorService.Aggregate(result);
            return Ok(output);
        }

        [HttpGet("result")]
        public IActionResult Result()
        {
            return Ok(AggregatorService.GetResult());
        }
    }
}