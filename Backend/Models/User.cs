using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class User
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<ResumeUpload> ResumeUploads { get; set; } = new List<ResumeUpload>();
}
