using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class AIAnalysis
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // FK must match ResumeUpload.Id type (Guid)
        [Required]
        public Guid ResumeUploadId { get; set; }

        [ForeignKey(nameof(ResumeUploadId))]
        public ResumeUpload ResumeUpload { get; set; } = null!;

        
        [Required]
        public string Result { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
