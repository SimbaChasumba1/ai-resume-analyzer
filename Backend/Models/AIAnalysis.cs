public class AIAnalysis
{
    public Guid Id { get; set; }
    public Guid ResumeId { get; set; }
    public string Summary { get; set; }
    public string Skills { get; set; }
    public string Weaknesses { get; set; }
    public string Recommendations { get; set; }

    public ResumeUpload Resume { get; set; }
}
