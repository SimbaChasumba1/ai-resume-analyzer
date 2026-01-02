using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class AIAnalysis
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string ResumeUploadId { get; set; }  // FK to ResumeUpload

        [ForeignKey("ResumeUploadId")]
        public ResumeUpload ResumeUpload { get; set; } = null!;

        public string Result { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
