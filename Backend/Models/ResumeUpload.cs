using System;
using System.Collections.Generic;

namespace backend.Models
{
    public class ResumeUpload
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string ExtractedText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; } = null!;

        public ICollection<AIAnalysis> Analyses { get; set; }
            = new List<AIAnalysis>();
    }
}
