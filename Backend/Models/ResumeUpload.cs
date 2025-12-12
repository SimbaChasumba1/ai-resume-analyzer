using System;

namespace backend.Models
{
    public class ResumeUpload
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Uploaded";

        public AIAnalysis Analysis { get; set; }
    }
}

