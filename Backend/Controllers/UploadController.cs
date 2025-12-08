using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Models;

[ApiController]
[Route("upload")]
public class UploadController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public UploadController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpPost]
    public async Task<IActionResult> UploadResume(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        var resume = new ResumeUpload
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Empty, // TODO: Replace with logged-in user
            FilePath = file.FileName
        };

        _db.ResumeUploads.Add(resume);
        await _db.SaveChangesAsync();

        // Save to disk
        var savePath = Path.Combine(_env.ContentRootPath, "uploads", resume.Id + ".pdf");
        using (var stream = System.IO.File.Create(savePath))
        {
            await file.CopyToAsync(stream);
        }

        // Process with AI next (Day 3)
        return Ok(new { resumeId = resume.Id, message = "Upload saved" });
    }
}


