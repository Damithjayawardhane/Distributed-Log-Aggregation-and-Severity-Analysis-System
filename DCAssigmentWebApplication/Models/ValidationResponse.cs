namespace WebApplicationDC.Models
{
    public class ValidationResponse
    {
        public string Vote { get; set; } = "";

        public int AcceptVotes { get; set; }

        public int ValidatorCount { get; set; }

        public int RequiredQuorum { get; set; }
    }
}