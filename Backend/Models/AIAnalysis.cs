namespace backend.Models
{
    public class AIAnalysis
    {
        public int Id { get; set; }

        public string Summary { get; set; } = string.Empty;
        public string Strengths { get; set; } = string.Empty;
        public string Weaknesses { get; set; } = string.Empty;
        public string Suggestions { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ResumeUploadId { get; set; }
        public ResumeUpload Resume { get; set; } = null!;
    }
}
