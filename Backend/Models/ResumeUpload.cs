public class ResumeUpload
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; }
    public AIAnalysis? Analysis { get; set; }
}
