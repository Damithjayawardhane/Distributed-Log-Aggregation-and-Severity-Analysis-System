using WebApplicationDC.Models;

namespace DCAssigmentWebApplication.Services
{
    public static class AggregatorService
    {
        private static readonly object LockObj = new();

        private static readonly HashSet<string> ProcessedChunkIds = new(StringComparer.Ordinal);

        private static Dictionary<string, int> GlobalCounts = new()
        {
            ["INFO"] = 0,
            ["DEBUG"] = 0,
            ["WARNING"] = 0,
            ["ERROR"] = 0,
            ["CRITICAL"] = 0
        };

        public static Dictionary<string, int> Aggregate(SeverityResult result)
        {
            lock (LockObj)
            {
                if (!string.IsNullOrEmpty(result.ChunkId) && !ProcessedChunkIds.Add(result.ChunkId))
                    return Snapshot(GlobalCounts);

                foreach (var item in result.Counts)
                    GlobalCounts[item.Key] += item.Value;

                return Snapshot(GlobalCounts);
            }
        }

        public static Dictionary<string, int> GetResult()
        {
            lock (LockObj)
                return Snapshot(GlobalCounts);
        }

        private static Dictionary<string, int> Snapshot(Dictionary<string, int> source)
        {
            return source.ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
}
