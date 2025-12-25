using System;

namespace backend.Models
{
    public class AIAnalysis
    {
        public Guid Id { get; set; }

        public Guid ResumeUploadId { get; set; }

        public string ResultJson { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ResumeUpload Resume { get; set; } = null!;
    }
}
