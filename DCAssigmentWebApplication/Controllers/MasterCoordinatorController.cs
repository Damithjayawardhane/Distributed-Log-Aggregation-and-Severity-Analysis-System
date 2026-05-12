using Microsoft.AspNetCore.Mvc;
using WebApplicationDC.Models;
using System.Text.RegularExpressions;
using DCAssigmentWebApplication.Services;

namespace WebApplicationDC.Controllers
{
    [ApiController]
    [Route("masterCoordinator")]

    public class MasterCoordinatorController : ControllerBase
    {
        [HttpPost("start")]
        public IActionResult Start()
        {
            Console.WriteLine("Coordinator started...");

            var logs = System.IO.File.ReadAllLines("logs.txt").ToList();

            var chunk = new LogChunk
            {
                ChunkId = Guid.NewGuid().ToString(),
                Logs = logs
            };

            // Mapper
            var mapperResult = MapperService.Process(chunk);

            if (mapperResult == null)
                return BadRequest("Mapper failed");

            // Validator
            var validationResult = ValidatorService.Validate(chunk, mapperResult);

            if (validationResult.Vote != "ACCEPT")
                return BadRequest("Validation failed");

            // Aggregator
            var aggregated = AggregatorService.Aggregate(mapperResult);

            return Ok(new
            {
                message = "Distributed log processing completed successfully",
                chunkId = chunk.ChunkId,
                mapperResult = mapperResult.Counts,
                validation = validationResult.Vote,
                finalResult = aggregated
            });
        }
    }
}