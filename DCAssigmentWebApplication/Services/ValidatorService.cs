using WebApplicationDC.Models;

namespace DCAssigmentWebApplication.Services
{
    public static class ValidatorService
    {
        public static ValidationResponse Validate(LogChunk chunk, SeverityResult mapperResult)
        {
            var recomputed = MapperService.Process(chunk);

            bool same = recomputed.Counts.OrderBy(x => x.Key)
            .SequenceEqual(mapperResult.Counts.OrderBy(x => x.Key));

            return new ValidationResponse
            {
                Vote = same ? "ACCEPT" : "REJECT"
            };
        }
    }
}
