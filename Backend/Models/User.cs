using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class User
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }


    public ICollection<ResumeUpload> Resumes { get; set; } = new List<ResumeUpload>();
}
