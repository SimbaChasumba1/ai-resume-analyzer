public class ResumeUpload
{
public int Id { get; set; }
public string FileName { get; set; } = string.Empty;
public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
public string Status { get; set; } = "Queued";


public int UserId { get; set; }
public User User { get; set; } = null!;


public AIAnalysis? Analysis { get; set; }
}