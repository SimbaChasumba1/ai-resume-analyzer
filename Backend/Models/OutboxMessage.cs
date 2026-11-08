using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class OutboxMessage
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Type { get; set; } = string.Empty;

    [Required]
    public string Payload { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ProcessedAt { get; set; }

    public DateTime? NextAttemptAt { get; set; }

    public int Attempts { get; set; } = 0;

    public string? LastError { get; set; }
}