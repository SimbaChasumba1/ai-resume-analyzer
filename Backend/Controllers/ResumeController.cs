using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResumesController : ControllerBase
    {
        private readonly ResumeParser _parser;
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ResumesController(ResumeParser parser, AppDbContext db, IWebHostEnvironment env)
        {
            _parser = parser; _db = db; _env = env;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");
            if (!file.FileName.EndsWith(".pdf") && !file.FileName.EndsWith(".docx")) return BadRequest("Only PDF/DOCX allowed.");
            if (file.Length > 5 * 1024 * 1024) return BadRequest("Max 5MB.");

            var uploads = Path.Combine(_env.ContentRootPath, "Uploads");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

            var safeName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploads, safeName);

            await using (var stream = System.IO.File.Create(filePath)) await file.CopyToAsync(stream);

            var skills = _parser.ExtractSkillsFromResume(filePath);

            var resume = new Resume { OriginalFileName = file.FileName, FilePath = filePath };
            _db.Resumes.Add(resume);
            await _db.SaveChangesAsync();

            return Ok(new { id = resume.Id, skills });
        }
    }
}
