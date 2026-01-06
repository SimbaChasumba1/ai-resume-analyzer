using System.Security.Claims;
using System.Text.Json;
using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace backend.Controllers
{
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
            ILogger<ResumeController> logger
        )
        {
            _db = db;
            _pdf = pdf;
            _ai = ai;
            _logger = logger;
        }

        // ================================
        // UPLOAD + ANALYZE
        // ================================
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Upload failed: no file provided.");
                return BadRequest("No file uploaded");
            }

            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
            {
                _logger.LogWarning("Upload failed: invalid user ID.");
                return Unauthorized("Invalid user");
            }

            string resumeText;
            try
            {
                using var stream = file.OpenReadStream();
                resumeText = _pdf.ExtractText(stream);

                if (string.IsNullOrWhiteSpace(resumeText))
                {
                    _logger.LogWarning("PDF extraction returned empty text for file {FileName}", file.FileName);
                    return BadRequest("Cannot extract text from this PDF");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF extraction failed for file {FileName}", file.FileName);
                return StatusCode(500, "Failed to extract resume text");
            }

            ResumeAnalysisResult analysis;
            try
            {
                analysis = await _ai.AnalyzeAsync(resumeText);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI analysis failed for file {FileName}", file.FileName);
                return StatusCode(500, "AI analysis failed: see server logs for details");
            }

            var entity = new AIAnalysis
            {
                UserId = userId,
                ResumeFileName = file.FileName,
                Summary = analysis.Summary,
                AtsScore = analysis.AtsScore,
                StrengthsJson = JsonSerializer.Serialize(analysis.Strengths),
                WeaknessesJson = JsonSerializer.Serialize(analysis.Weaknesses),
                ImprovementsJson = JsonSerializer.Serialize(analysis.Improvements),
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                _db.AIAnalyses.Add(entity);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database save failed for file {FileName}", file.FileName);
                return StatusCode(500, "Failed to save analysis");
            }

            _logger.LogInformation("Resume uploaded and analyzed successfully: {FileName}, user {UserId}", file.FileName, userId);
            return Ok(new { analysisId = entity.Id });
        }

        // ================================
        // DASHBOARD — GET MY ANALYSES
        // ================================
        [HttpGet("my")]
        public async Task<IActionResult> GetMyAnalyses()
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
                return Unauthorized("Invalid user");

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

        // ================================
        // GET ANALYSIS BY ID
        // ================================
        [HttpGet("analysis/{id:guid}")]
        public async Task<IActionResult> GetAnalysis(Guid id)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
                return Unauthorized("Invalid user");

            var analysis = await _db.AIAnalyses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (analysis == null)
                return NotFound("Analysis not found");

            return Ok(new
            {
                analysis.Id,
                analysis.ResumeFileName,
                analysis.Summary,
                analysis.AtsScore,
                Strengths = JsonSerializer.Deserialize<string[]>(analysis.StrengthsJson),
                Weaknesses = JsonSerializer.Deserialize<string[]>(analysis.WeaknessesJson),
                Improvements = JsonSerializer.Deserialize<string[]>(analysis.ImprovementsJson),
                analysis.CreatedAt
            });
        }
    }
}
