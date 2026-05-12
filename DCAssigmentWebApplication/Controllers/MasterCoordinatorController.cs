using DCAssigmentWebApplication.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplicationDC.Models;

namespace WebApplicationDC.Controllers
{
    [ApiController]
    [Route("masterCoordinator")]
    public class MasterCoordinatorController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public MasterCoordinatorController(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        [HttpPost("start")]
        public IActionResult Start()
        {
            var logPath = ResolveLogPath();
            if (!System.IO.File.Exists(logPath))
                return BadRequest($"Log file not found: {logPath}");

            var lines = System.IO.File.ReadAllLines(logPath).ToList();
            var runId = Guid.NewGuid().ToString("N");
            var mapperShards = Math.Max(1, _configuration.GetValue("LogProcessing:MapperShards", 4));
            var validatorNodes = Math.Max(2, _configuration.GetValue("LogProcessing:ValidatorNodes", 3));

            var chunks = PartitionLineRanges(lines, runId, mapperShards);
            ValidationResponse? lastValidation = null;

            foreach (var chunk in chunks)
            {
                var mapperResult = MapperService.Process(chunk);
                var validationResult = ValidatorService.ValidateQuorum(chunk, mapperResult, validatorNodes);
                if (validationResult.Vote != "ACCEPT")
                {
                    return BadRequest(new
                    {
                        message = "Validation failed for a shard",
                        chunkId = chunk.ChunkId,
                        validation = validationResult
                    });
                }

                AggregatorService.Aggregate(mapperResult);
                lastValidation = validationResult;
            }

            var aggregated = AggregatorService.GetResult();

            return Ok(new
            {
                message = "OK",
                validation = lastValidation!.Vote,
                runId,
                mapperShards = chunks.Count,
                validatorNodes,
                acceptVotes = lastValidation.AcceptVotes,
                requiredQuorum = lastValidation.RequiredQuorum,
                finalResult = aggregated
            });
        }

        private string ResolveLogPath()
        {
            var configured = _configuration["LogProcessing:LogFilePath"];
            if (!string.IsNullOrWhiteSpace(configured))
            {
                if (Path.IsPathRooted(configured))
                    return configured;
                return Path.GetFullPath(Path.Combine(_env.ContentRootPath, configured));
            }

            return Path.Combine(_env.ContentRootPath, "logs.txt");
        }

        private static List<LogChunk> PartitionLineRanges(List<string> lines, string runId, int mapperCount)
        {
            mapperCount = Math.Min(Math.Max(1, mapperCount), Math.Max(1, lines.Count));
            var chunks = new List<LogChunk>();
            var n = lines.Count;
            if (n == 0)
            {
                chunks.Add(new LogChunk { ChunkId = $"{runId}-m0", Logs = new List<string>() });
                return chunks;
            }

            var baseSize = n / mapperCount;
            var remainder = n % mapperCount;
            var start = 0;
            for (var i = 0; i < mapperCount; i++)
            {
                var take = baseSize + (i < remainder ? 1 : 0);
                if (take <= 0)
                    continue;

                var slice = lines.GetRange(start, take);
                chunks.Add(new LogChunk
                {
                    ChunkId = $"{runId}-m{i}",
                    Logs = slice
                });
                start += take;
            }

            return chunks;
        }
    }
}
