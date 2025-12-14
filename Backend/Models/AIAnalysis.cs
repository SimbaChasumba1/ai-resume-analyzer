using System;


namespace backend.Models;


public class AIAnalysis
{
public Guid Id { get; set; }
public Guid ResumeUploadId { get; set; }


public string Summary { get; set; } = string.Empty;
public string Skills { get; set; } = string.Empty;
public string Weaknesses { get; set; } = string.Empty;
public string Recommendations { get; set; } = string.Empty;


public ResumeUpload? Resume { get; set; }
}