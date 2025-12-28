using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("resume")]
[Authorize]
public class ResumeController : ControllerBase
{
    private readonly AppDbContext _db;

    public ResumeController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var userId = User.Claims.First(c => c.Type == "sub").Value;

        var resume = new ResumeUpload
        {
            UserId = Guid.Parse(userId),
            FileName = file.FileName
        };

        _db.ResumeUploads.Add(resume);
        await _db.SaveChangesAsync();

        return Ok();
    }
}
