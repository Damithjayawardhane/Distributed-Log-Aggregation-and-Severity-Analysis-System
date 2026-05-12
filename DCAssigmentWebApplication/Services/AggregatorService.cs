using WebApplicationDC.Models;

namespace DCAssigmentWebApplication.Services
{
    public static class AggregatorService
    {
        private static readonly object _lock = new();

        private static Dictionary<string, int> globalCounts = new()
        {
            ["INFO"] = 0,
            ["DEBUG"] = 0,
            ["WARNING"] = 0,
            ["ERROR"] = 0,
            ["CRITICAL"] = 0
        };

        public static Dictionary<string, int> Aggregate(SeverityResult result)
        {
            lock (_lock)
            {
                foreach (var item in result.Counts)
                {
                    globalCounts[item.Key] += item.Value;
                }
            }

            return globalCounts;
        }

        public static Dictionary<string, int> GetResult()
        {
            return globalCounts;
        }
    }
}