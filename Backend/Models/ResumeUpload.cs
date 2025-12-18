using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class ResumeUpload
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        public string ExtractedText { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<AIAnalysis> Analyses { get; set; } = new List<AIAnalysis>();
    }
}
