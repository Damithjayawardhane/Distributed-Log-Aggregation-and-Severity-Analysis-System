namespace WebApplicationDC.Models
{
    public class SeverityResult
    {
        public string ChunkId { get; set; } = "";

        public Dictionary<string, int> Counts { get; set; } = new();

        public Dictionary<string, List<string>> UniqueMessages { get; set; } = new();
    }
}