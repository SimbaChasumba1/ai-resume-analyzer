using System.Security.Claims;
using System.Text.Json;
using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace backend.Controllers;

[ApiController]
[Route("api/resume")]
[Authorize]
public class ResumeController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPdfTextExtractor _pdf;
    private readonly IResumeAnalysisService _ai;
    private readonly ILogger<ResumeController> _logger;

    public ResumeController(
        AppDbContext db,
        IPdfTextExtractor pdf,
        IResumeAnalysisService ai,
        ILogger<ResumeController> logger)
    {
        _db = db;
        _pdf = pdf;
        _ai = ai;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        string resumeText;
        try
        {
            using var stream = file.OpenReadStream();
            resumeText = _pdf.ExtractText(stream);

            if (string.IsNullOrWhiteSpace(resumeText))
                return BadRequest("Unable to extract text from resume.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PDF extraction failed");
            return StatusCode(500, "Failed to read resume.");
        }

        ResumeAnalysisResult analysis;

        try
        {
            analysis = await _ai.AnalyzeAsync(resumeText);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI unavailable — returning demo analysis");

            analysis = new ResumeAnalysisResult
            {
                Summary = "Demo AI analysis (AI temporarily unavailable).",
                AtsScore = 75,
                Strengths = new()
                {
                    "Strong technical foundation",
                    "Relevant project experience",
                    "Clear resume structure"
                },
                Weaknesses = new()
                {
                    "Some achievements lack metrics",
                    "Skills section could be more targeted"
                },
                Improvements = new()
                {
                    "Quantify results with numbers",
                    "Customize keywords per job role"
                },
                CreatedAt = DateTime.UtcNow
            };
        }

        var entity = new AIAnalysis
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ResumeFileName = file.FileName,
            Summary = analysis.Summary,
            AtsScore = analysis.AtsScore,
            StrengthsJson = JsonSerializer.Serialize(analysis.Strengths),
            WeaknessesJson = JsonSerializer.Serialize(analysis.Weaknesses),
            ImprovementsJson = JsonSerializer.Serialize(analysis.Improvements),
            CreatedAt = DateTime.UtcNow
            // 🚫 ResumeUploadId REMOVED — avoids FK violation
        };

        try
        {
            _db.AIAnalyses.Add(entity);
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database save failed");
            return StatusCode(500, "Failed to save analysis.");
        }

        return Ok(new
        {
            analysisId = entity.Id,
            demo = analysis.Summary.StartsWith("Demo")
        });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyAnalyses()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var analyses = await _db.AIAnalyses
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new
            {
                a.Id,
                a.ResumeFileName,
                a.AtsScore,
                a.Summary,
                a.CreatedAt
            })
            .ToListAsync();

        return Ok(analyses);
    }

    [HttpGet("analysis/{id:guid}")]
    public async Task<IActionResult> GetAnalysis(Guid id)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var analysis = await _db.AIAnalyses
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        if (analysis == null)
            return NotFound();

        return Ok(new
        {
            analysis.Id,
            analysis.ResumeFileName,
            analysis.Summary,
            analysis.AtsScore,
            Strengths = JsonSerializer.Deserialize<List<string>>(analysis.StrengthsJson),
            Weaknesses = JsonSerializer.Deserialize<List<string>>(analysis.WeaknessesJson),
            Improvements = JsonSerializer.Deserialize<List<string>>(analysis.ImprovementsJson),
            analysis.CreatedAt
        });
    }

    // ✅ DELETE endpoint for real deletion
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAnalysis(Guid id)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var analysis = await _db.AIAnalyses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (analysis == null)
            return NotFound("Analysis not found.");

        _db.AIAnalyses.Remove(analysis);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
