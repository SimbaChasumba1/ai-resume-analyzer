namespace Backend.Models
{
    public class Resume
    {
        public int Id { get; set; }
        public string? OriginalFileName { get; set; }
        public string? FilePath { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
