using backend.Models;

namespace backend.Services;

public interface IResumeAnalysisService
{
    Task<ResumeAnalysisResult> AnalyzeAsync(string resumeText);
}
