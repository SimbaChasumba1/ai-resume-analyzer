using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/resume")]
    [Authorize]
    public class ResumeController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ResumeController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // Get current user ID from JWT claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID");

            // Ensure uploads directory exists
            var uploadsDir = Path.Combine(_env.ContentRootPath, "uploads");
            Directory.CreateDirectory(uploadsDir);

            // Save file
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsDir, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Create resume entity
            var resume = new ResumeUpload
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                FilePath = filePath,
                ExtractedText = "", // filled later by AI
                UserId = userId // Guid type
            };

            // Add and save
            _db.ResumeUploads.Add(resume);
            await _db.SaveChangesAsync();

            return Ok(new { resume.Id });
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyResumes()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID");

            var resumes = _db.ResumeUploads
                .Where(r => r.UserId == userId)
                .Select(r => new
                {
                    r.Id,
                    r.FileName,
                    r.CreatedAt
                })
                .ToList();

            return Ok(resumes);
        }
    }
}
