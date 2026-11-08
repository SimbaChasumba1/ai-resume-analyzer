using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly ILogger<ResumeController> _logger;

        public ResumeController(
            AppDbContext db,
            IPdfTextExtractor pdf,
            ILogger<ResumeController> logger)
        {
            _db = db;
            _pdf = pdf;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadResume(IFormFile file)
        {
            if (!Guid.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
            {
                return Unauthorized();
            }

          const long maxFileSize = 10 * 1024 * 1024;

if (file == null || file.Length == 0)
    return BadRequest("Please upload a valid PDF file.");

if (file.Length > maxFileSize)
    return BadRequest("PDF file size cannot exceed 10 MB.");

if (!string.Equals(
    Path.GetExtension(file.FileName),
    ".pdf",
    StringComparison.OrdinalIgnoreCase))
{
    return BadRequest("Only PDF files are supported.");
}

byte[] pdfSignature = new byte[5];

using (var signatureStream = file.OpenReadStream())
{
    var bytesRead = await signatureStream.ReadAsync(
        pdfSignature, 0, pdfSignature.Length);

    if (bytesRead != 5 ||
        !pdfSignature.SequenceEqual(
            new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }))
    {
        return BadRequest("The uploaded file is not a valid PDF.");
    }
}
            var userExists = await _db.Users
                .AnyAsync(u => u.Id == userId);

            if (!userExists)
                return Unauthorized();

            string resumeText;

            try
            {
                using var stream = file.OpenReadStream();
                resumeText = _pdf.ExtractText(stream);

                if (string.IsNullOrWhiteSpace(resumeText))
                    return BadRequest("Could not extract text from this PDF.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to extract resume text.");
                return BadRequest("Unable to read the uploaded PDF.");
            }

            var now = DateTime.UtcNow;

            var resumeUpload = new ResumeUpload
            {
                Id = Guid.NewGuid(),
                FileName = Path.GetFileName(file.FileName),
                FilePath = Path.GetFileName(file.FileName),
                ExtractedText = resumeText,
                UserId = userId,
                CreatedAt = now
            };

            var analysisJob = new AnalysisJob
            {
                Id = Guid.NewGuid(),
                ResumeUploadId = resumeUpload.Id,
                UserId = userId,
                Status = "Pending",
                RetryCount = 0,
                CreatedAt = now
            };

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "ResumeAnalysisRequested",

                Payload = JsonSerializer.Serialize(
                    new AnalysisJobMessage
                    {
                        JobId = analysisJob.Id
                    }),

                CreatedAt = now,
                Attempts = 0
            };

            // Save all three records together.
            _db.ResumeUploads.Add(resumeUpload);
            _db.AnalysisJobs.Add(analysisJob);
            _db.OutboxMessages.Add(outboxMessage);

            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Resume uploaded. Analysis job {JobId} and outbox message {OutboxId} created.",
                analysisJob.Id,
                outboxMessage.Id);

            return AcceptedAtAction(
                nameof(GetJobStatus),
                new { id = analysisJob.Id },
                new
                {
                    message = "Resume uploaded successfully. Analysis is pending.",
                    jobId = analysisJob.Id,
                    status = analysisJob.Status
                });
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyResumes()
        {
            if (!Guid.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
            {
                return Unauthorized();
            }

            var jobs = await _db.AnalysisJobs
                .AsNoTracking()
                .Include(j => j.ResumeUpload)
                .Include(j => j.Analysis)
                .Where(j => j.UserId == userId)
                .OrderByDescending(j => j.CreatedAt)
                .Select(j => new
                {
                    JobId = j.Id,
                    ResumeFileName = j.ResumeUpload.FileName,
                    Status = j.Status,
                    CreatedAt = j.CreatedAt,
                    CompletedAt = j.CompletedAt,
                    AnalysisId = j.Analysis != null
                        ? j.Analysis.Id
                        : (Guid?)null,
                    AtsScore = j.Analysis != null
                        ? j.Analysis.AtsScore
                        : (int?)null
                })
                .ToListAsync();

            return Ok(jobs);
        }

        [HttpGet("analysis/{id:guid}")]
        public async Task<IActionResult> GetJobStatus(Guid id)
        {
            if (!Guid.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
            {
                return Unauthorized();
            }

            var job = await _db.AnalysisJobs
                .AsNoTracking()
                .Include(j => j.ResumeUpload)
                .Include(j => j.Analysis)
                .FirstOrDefaultAsync(
                    j => j.Id == id && j.UserId == userId);

            if (job == null)
                return NotFound("Analysis job not found.");

            object? analysis = null;

            if (job.Analysis != null)
            {
                analysis = new
                {
                    job.Analysis.Id,
                    job.Analysis.ResumeFileName,
                    job.Analysis.AtsScore,
                    job.Analysis.Summary,

                    Strengths = DeserializeList(
                        job.Analysis.StrengthsJson),

                    Weaknesses = DeserializeList(
                        job.Analysis.WeaknessesJson),

                    Improvements = DeserializeList(
                        job.Analysis.ImprovementsJson)
                };
            }

            return Ok(new
            {
                jobId = job.Id,
                resumeFileName = job.ResumeUpload.FileName,
                status = job.Status,
                createdAt = job.CreatedAt,
                completedAt = job.CompletedAt,
                errorMessage = job.ErrorMessage,
                analysis
            });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteResume(Guid id)
        {
            if (!Guid.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
            {
                return Unauthorized();
            }

            var job = await _db.AnalysisJobs
                .Include(j => j.ResumeUpload)
                .FirstOrDefaultAsync(
                    j => j.Id == id && j.UserId == userId);

            if (job == null)
                return NotFound("Resume job not found.");

            _db.AnalysisJobs.Remove(job);
            _db.ResumeUploads.Remove(job.ResumeUpload);

            await _db.SaveChangesAsync();

            return NoContent();
        }

        private static List<string> DeserializeList(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<List<string>>(json)
                    ?? new List<string>();
            }
            catch (JsonException)
            {
                return new List<string>();
            }
        }
    }
}