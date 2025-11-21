using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers
{
    [ApiController]
    [Route("resumes")]
    public class ResumeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ResumeController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var uploads = Path.Combine(_env.ContentRootPath, "Uploads");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, file.FileName);

            using (var stream = System.IO.File.Create(filePath))
                await file.CopyToAsync(stream);

            var resume = new Resume
            {
                OriginalFileName = file.FileName,
                FilePath = filePath
            };

            _context.Resumes.Add(resume);
            await _context.SaveChangesAsync();

            return Ok(new { id = resume.Id });
        }
    }
}
