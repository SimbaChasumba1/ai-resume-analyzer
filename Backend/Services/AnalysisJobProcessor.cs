using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace backend.Services;

public class AnalysisJobProcessor
{
    private const int MaxRetries = 3;

    private readonly AppDbContext _db;
    private readonly IResumeAnalysisService _analysisService;
    private readonly ILogger<AnalysisJobProcessor> _logger;

    public AnalysisJobProcessor(
        AppDbContext db,
        IResumeAnalysisService analysisService,
        ILogger<AnalysisJobProcessor> logger)
    {
        _db = db;
        _analysisService = analysisService;
        _logger = logger;
    }

    public async Task<bool> ProcessJobAsync(
        Guid jobId,
        CancellationToken cancellationToken)
    {
        var job = await _db.AnalysisJobs
            .Include(j => j.ResumeUpload)
            .Include(j => j.Analysis)
            .FirstOrDefaultAsync(
                j => j.Id == jobId,
                cancellationToken);

        if (job == null)
        {
            _logger.LogWarning(
                "Analysis job {JobId} was not found.",
                jobId);

            return false;
        }

        if (job.Status == "Completed")
        {
            _logger.LogInformation(
                "Analysis job {JobId} is already completed.",
                jobId);

            return false;
        }

        if (job.Status == "Failed")
        {
            _logger.LogWarning(
                "Analysis job {JobId} has already failed.",
                jobId);

            return false;
        }

        if (job.Analysis != null)
        {
            job.Status = "Completed";
            job.CompletedAt ??= DateTime.UtcNow;

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Analysis job {JobId} already has an analysis. Marked as completed.",
                jobId);

            return false;
        }

        try
        {
            job.Status = "Processing";
            job.StartedAt = DateTime.UtcNow;
            job.ErrorMessage = null;

            await _db.SaveChangesAsync(cancellationToken);

            var resumeText = job.ResumeUpload.ExtractedText;

            if (string.IsNullOrWhiteSpace(resumeText))
            {
                throw new InvalidOperationException(
                    "The uploaded resume contains no extracted text.");
            }

            var result = await _analysisService.AnalyzeAsync(resumeText);

            if (result == null)
            {
                throw new InvalidOperationException(
                    "The Python service returned an empty response.");
            }

            var analysis = new AIAnalysis
            {
                Id = Guid.NewGuid(),
                AnalysisJobId = job.Id,
                ResumeFileName = job.ResumeUpload.FileName,
                AtsScore = result.AtsScore,
                Summary = result.Summary,
                StrengthsJson = System.Text.Json.JsonSerializer.Serialize(
                    result.Strengths),
                WeaknessesJson = System.Text.Json.JsonSerializer.Serialize(
                    result.Weaknesses),
                ImprovementsJson = System.Text.Json.JsonSerializer.Serialize(
                    result.Improvements),
                CreatedAt = DateTime.UtcNow
            };

            _db.AIAnalyses.Add(analysis);

            job.Status = "Completed";
            job.CompletedAt = DateTime.UtcNow;
            job.ErrorMessage = null;

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Analysis job {JobId} completed successfully.",
                jobId);

            return false;
        }
        catch (OperationCanceledException) when (
            cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            job.RetryCount++;
            job.ErrorMessage = ex.Message;

            var shouldRetry = job.RetryCount < MaxRetries;

            job.Status = shouldRetry ? "Pending" : "Failed";

            if (!shouldRetry)
            {
                job.CompletedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogError(
                ex,
                "Analysis job {JobId} failed on attempt {RetryCount}. Retry: {ShouldRetry}",
                jobId,
                job.RetryCount,
                shouldRetry);

            return shouldRetry;
        }
    }
}