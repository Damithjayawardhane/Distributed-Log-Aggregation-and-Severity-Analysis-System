namespace WebApplicationDC.Models
{
    public class ValidationRequest
    {
        public LogChunk Chunk { get; set; } = new();

        public SeverityResult MapperResult { get; set; } = new();
    }
}