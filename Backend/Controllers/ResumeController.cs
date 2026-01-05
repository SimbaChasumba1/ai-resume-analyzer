using System.Security.Claims;
using System.Text.Json;
using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;  // Add logging support

namespace backend.Controllers
{
    [ApiController]
    [Route("api/resume")]
    public class ResumeController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPdfTextExtractor _pdf;
        private readonly IResumeAnalysisService _ai;
        private readonly ILogger<ResumeController> _logger; // Logger injection

        // Constructor with Logger injection
        public ResumeController(
            AppDbContext db,
            IPdfTextExtractor pdf,
            IResumeAnalysisService ai,
            ILogger<ResumeController> logger  // Logger parameter
        )
        {
            _db = db;
            _pdf = pdf;
            _ai = ai;
            _logger = logger;  // Assigning logger
        }

        // 🔹 STEP 4 — UPLOAD + ANALYZE
        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // Check if a file is uploaded
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No file uploaded.");
                return BadRequest("No file uploaded");
            }

            // Retrieve current user ID from JWT claims
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            string resumeText;
            try
            {
                // Extract text from the PDF
                using (var stream = file.OpenReadStream())
                {
                    resumeText = _pdf.ExtractText(stream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting text from resume.");
                return StatusCode(500, "Failed to extract text from the PDF file.");
            }

            // Perform AI analysis
            ResumeAnalysisResult analysis;
            try
            {
                analysis = await _ai.AnalyzeAsync(resumeText);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing resume text.");
                return StatusCode(500, "AI analysis failed.");
            }

            // Create a new entity to store the analysis results
            var entity = new AIAnalysis
            {
                UserId = userId,
                ResumeFileName = file.FileName,
                Summary = analysis.Summary,
                AtsScore = analysis.AtsScore,
                StrengthsJson = JsonSerializer.Serialize(analysis.Strengths),
                WeaknessesJson = JsonSerializer.Serialize(analysis.Weaknesses),
                ImprovementsJson = JsonSerializer.Serialize(analysis.ImprovementSuggestions)
            };

            try
            {
                // Save the entity to the database
                _db.AIAnalyses.Add(entity);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving analysis to the database.");
                return StatusCode(500, "Failed to save analysis.");
            }

            // Return analysis ID
            _logger.LogInformation("Resume uploaded and analyzed successfully for user {UserId}.", userId);
            return Ok(new { analysisId = entity.Id });
        }

        // 🔹 STEP 6 — FETCH ANALYSIS BY ID
        [Authorize]
        [HttpGet("analysis/{id}")]
        public async Task<IActionResult> GetAnalysis(int id)
        {
            // Retrieve current user ID from JWT claims
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            AIAnalysis? analysis;
            try
            {
                // Query the analysis by ID and ensure it belongs to the current user
                analysis = await _db.AIAnalyses
                    .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching analysis from the database.");
                return StatusCode(500, "Failed to fetch analysis.");
            }

            // If no analysis found, return a NotFound status
            if (analysis == null)
            {
                _logger.LogWarning("Analysis not found for ID {Id} and User {UserId}.", id, userId);
                return NotFound("Analysis not found");
            }

            // Deserialize JSON fields (Strengths, Weaknesses, Improvements)
            var result = new
            {
                analysis.Id,
                analysis.ResumeFileName,
                analysis.Summary,
                analysis.AtsScore,
                Strengths = JsonSerializer.Deserialize<string[]>(analysis.StrengthsJson),
                Weaknesses = JsonSerializer.Deserialize<string[]>(analysis.WeaknessesJson),
                Improvements = JsonSerializer.Deserialize<string[]>(analysis.ImprovementsJson),
                analysis.CreatedAt
            };

            // Log successful retrieval
            _logger.LogInformation("Analysis {Id} retrieved successfully for user {UserId}.", id, userId);

            return Ok(result);
        }
    }
}
