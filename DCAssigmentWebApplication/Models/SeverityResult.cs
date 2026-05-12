namespace WebApplicationDC.Models
{
    public class SeverityResult
    {
        public string ChunkId { get; set; } = "";

        public Dictionary<string, int> Counts { get; set; } = new();
    }
}