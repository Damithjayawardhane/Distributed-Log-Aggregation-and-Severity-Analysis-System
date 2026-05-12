using System.Text.RegularExpressions;
using WebApplicationDC.Models;

namespace DCAssigmentWebApplication.Services
{
    public static class MapperService
    {
        public static SeverityResult Process(LogChunk chunk)
        {
            var counts = Init();

            var pattern = new Regex(@"\b(INFO|DEBUG|WARNING|WARN|ERROR|CRITICAL|FATAL)\b",
            RegexOptions.IgnoreCase);

            foreach (var line in chunk.Logs)
            {
                var match = pattern.Match(line);

                if (!match.Success) continue;

                var severity = Normalize(match.Value);

                counts[severity]++;
            }

            return new SeverityResult
            {
                ChunkId = chunk.ChunkId,
                Counts = counts
            };
        }

        private static Dictionary<string, int> Init() => new()
        {
            ["INFO"] = 0,
            ["DEBUG"] = 0,
            ["WARNING"] = 0,
            ["ERROR"] = 0,
            ["CRITICAL"] = 0
        };

        private static string Normalize(string value)
        {
            value = value.ToUpper();

            return value switch
            {
                "WARN" => "WARNING",
                "FATAL" => "CRITICAL",
                _ => value
            };
        }
    }
}

