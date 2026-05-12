namespace WebApplicationDC.Models
{
    public class LogChunk
    {
        public string ChunkId { get; set; } = "";
        public List<string> Logs { get; set; } = new();
    }
}