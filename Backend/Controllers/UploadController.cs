using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
    [Authorize] // ensure only logged-in users can upload
    public async Task<IActionResult> UploadResume(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        // Get logged-in user ID from JWT claim
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("User not authenticated");

        if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            return BadRequest("Invalid user ID");

        // Create resume entry
        var resume = new ResumeUpload
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FilePath = file.FileName,
            CreatedAt = DateTime.UtcNow,
            Status = "Pending"
        };

        // Save to DB first
        _db.ResumeUploads.Add(resume);
        await _db.SaveChangesAsync();

        // Save file to disk
        var uploadsFolder = Path.Combine(_env.ContentRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var savePath = Path.Combine(uploadsFolder, resume.Id + Path.GetExtension(file.FileName));
        using (var stream = System.IO.File.Create(savePath))
        {
            await file.CopyToAsync(stream);
        }

        // Optionally: trigger AI analysis in background here

        return Ok(new { resumeId = resume.Id, message = "Upload saved successfully" });
    }
}
