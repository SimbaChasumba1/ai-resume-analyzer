using System;

namespace backend.Models
{
    public class AIAnalysis
    {
        public Guid Id { get; set; }

        // This is the foreign key — AIAnalysis is dependent.
        public Guid ResumeUploadId { get; set; }

        public string Summary { get; set; }
        public string Skills { get; set; }
        public string Weaknesses { get; set; }
        public string Recommendations { get; set; }

        public ResumeUpload Resume { get; set; }
    }
}
