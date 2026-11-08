
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class AIAnalysis
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid AnalysisJobId { get; set; }

        [ForeignKey(nameof(AnalysisJobId))]
        public AnalysisJob AnalysisJob { get; set; } = null!;

        [Required]
        public string ResumeFileName { get; set; } = "";

        public string Summary { get; set; } = "";

        public string StrengthsJson { get; set; } = "";

        public string WeaknessesJson { get; set; } = "";

        public string ImprovementsJson { get; set; } = "";

        public int AtsScore { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
