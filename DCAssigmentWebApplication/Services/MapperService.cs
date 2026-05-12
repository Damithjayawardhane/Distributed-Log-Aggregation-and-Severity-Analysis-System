using System.Text.RegularExpressions;
using WebApplicationDC.Models;

namespace DCAssigmentWebApplication.Services
{
    public static class MapperService
    {
        private static readonly string[] Severities =
            ["INFO", "DEBUG", "WARNING", "ERROR", "CRITICAL"];

        private static readonly Regex Pattern = new(
            @"\b(INFO|DEBUG|WARNING|WARN|ERROR|CRITICAL|FATAL)\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static SeverityResult Process(LogChunk chunk)
        {
            var counts = Severities.ToDictionary(s => s, _ => 0);
            var uniqueSets = Severities.ToDictionary(
                s => s,
                _ => new HashSet<string>(StringComparer.Ordinal));

            foreach (var line in chunk.Logs)
            {
                var match = Pattern.Match(line);
                if (!match.Success) continue;

                var severity = Normalize(match.Value);
                counts[severity]++;
                uniqueSets[severity].Add(line.Trim());
            }

            return new SeverityResult
            {
                ChunkId = chunk.ChunkId,
                Counts = counts,
                UniqueMessages = uniqueSets.ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value.OrderBy(s => s, StringComparer.Ordinal).ToList(),
                    StringComparer.Ordinal)
            };
        }

        private static string Normalize(string value)
        {
            value = value.ToUpperInvariant();

            return value switch
            {
                "WARN" => "WARNING",
                "FATAL" => "CRITICAL",
                _ => value
            };
        }
    }
}
