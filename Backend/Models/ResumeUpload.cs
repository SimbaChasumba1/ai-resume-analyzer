using System;

using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;



namespace backend.Models

{

    public class ResumeUpload

    {

        [Key]

        public Guid Id { get; set; }



        [Required]

        public string FileName { get; set; } = string.Empty;



        [Required]

        public string FilePath { get; set; } = string.Empty;



        public string ExtractedText { get; set; } = string.Empty;



        [Required]

        public Guid UserId { get; set; }



        [ForeignKey(nameof(UserId))]

        public User User { get; set; } = null!;



        public AIAnalysis? Analysis { get; set; }



        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }

}



