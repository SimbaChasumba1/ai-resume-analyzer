namespace backend.Models
{
    public class ResumeAnalysis
    {
        public int Id { get; set; }
        public string ResumeText { get; set; } = string.Empty;
        public string AnalysisResult { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
