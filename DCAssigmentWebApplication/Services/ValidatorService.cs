using WebApplicationDC.Models;

namespace DCAssigmentWebApplication.Services
{
    public static class ValidatorService
    {
        /// <summary>Single validator: recompute from raw chunk and compare to mapper output.</summary>
        public static ValidationResponse Validate(LogChunk chunk, SeverityResult mapperResult)
        {
            var recomputed = MapperService.Process(chunk);
            var same = ResultsMatch(recomputed, mapperResult);
            return new ValidationResponse
            {
                Vote = same ? "ACCEPT" : "REJECT",
                AcceptVotes = same ? 1 : 0,
                ValidatorCount = 1,
                RequiredQuorum = 1
            };
        }

        /// <summary>
        /// Simulates N independent validators (assignment: ≥2, quorum floor(N/2)+1 ACCEPT).
        /// Each runs the same deterministic check; structure matches the report's voting model.
        /// </summary>
        public static ValidationResponse ValidateQuorum(LogChunk chunk, SeverityResult mapperResult, int validatorCount)
        {
            validatorCount = Math.Max(2, validatorCount);
            var single = Validate(chunk, mapperResult);
            var acceptVotes = single.Vote == "ACCEPT" ? validatorCount : 0;
            var requiredQuorum = validatorCount / 2 + 1;
            var quorumMet = acceptVotes >= requiredQuorum;

            return new ValidationResponse
            {
                Vote = quorumMet ? "ACCEPT" : "REJECT",
                AcceptVotes = acceptVotes,
                ValidatorCount = validatorCount,
                RequiredQuorum = requiredQuorum
            };
        }

        private static bool ResultsMatch(SeverityResult a, SeverityResult b)
        {
            if (!CountsMatch(a.Counts, b.Counts))
                return false;
            return UniqueMessagesMatch(a.UniqueMessages, b.UniqueMessages);
        }

        private static bool CountsMatch(Dictionary<string, int> x, Dictionary<string, int> y)
        {
            return x.OrderBy(p => p.Key).SequenceEqual(y.OrderBy(p => p.Key));
        }

        private static bool UniqueMessagesMatch(
            Dictionary<string, List<string>>? a,
            Dictionary<string, List<string>>? b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;

            var keys = a.Keys.Union(b.Keys, StringComparer.Ordinal).ToList();
            foreach (var key in keys)
            {
                var setA = new HashSet<string>(a.GetValueOrDefault(key) ?? new List<string>(), StringComparer.Ordinal);
                var setB = new HashSet<string>(b.GetValueOrDefault(key) ?? new List<string>(), StringComparer.Ordinal);
                if (!setA.SetEquals(setB))
                    return false;
            }

            return true;
        }
    }
}
