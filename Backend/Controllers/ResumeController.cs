using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

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

        // Endpoint to upload a resume
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // Define the uploads directory
            var uploads = Path.Combine(_env.ContentRootPath, "Uploads");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            // Save the file to the server
            var filePath = Path.Combine(uploads, file.FileName);
            using (var stream = System.IO.File.Create(filePath))
                await file.CopyToAsync(stream);

            // Save resume info in the database
            var resume = new Resume
            {
                OriginalFileName = file.FileName,
                FilePath = filePath
            };

            _context.Resumes.Add(resume);
            await _context.SaveChangesAsync();

            // Example: Returning dummy skills data for now (replace with actual logic)
            var skills = new[] { "JavaScript", "React", "C#" };

            return Ok(new { id = resume.Id, skills });
        }
    }
}
