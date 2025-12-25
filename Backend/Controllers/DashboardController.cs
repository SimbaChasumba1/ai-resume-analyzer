using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("dashboard")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetDashboard(Guid userId)
    {
        var resumes = await _db.ResumeUploads
            .Where(r => r.UserId == userId)
            .ToListAsync();

        return Ok(new
        {
            totalResumes = resumes.Count,
            latestUpload = resumes
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefault()
        });
    }
}
