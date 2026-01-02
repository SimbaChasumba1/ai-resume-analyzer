using System;

using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;



namespace backend.Models

{

    public class ResumeUpload

    {

        [Key]

        public Guid Id { get; set; } // PK as Guid



        [Required]

        public string FileName { get; set; }



        public string FilePath { get; set; }



        public string ExtractedText { get; set; } = "";



        // FK to User

        public Guid UserId { get; set; }  



        [ForeignKey("UserId")]

        public User User { get; set; } = null!;



        // Navigation property to AIAnalysis (1:1)

        public AIAnalysis? Analysis { get; set; }



        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }

}



