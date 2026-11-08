using System.Security.Claims;
using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("history")]
public class HistoryController : ControllerBase
{
    private readonly AppDbContext _db;

    public HistoryController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetHistory()
    {
        if (!Guid.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out var userId))
        {
            return Unauthorized();
        }

        var history = await _db.AIAnalyses
            .AsNoTracking()
            .Where(a => a.AnalysisJob.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new
            {
                a.Id,
                a.AnalysisJobId,
                a.ResumeFileName,
                a.Summary,
                a.StrengthsJson,
                a.WeaknessesJson,
                a.ImprovementsJson,
                a.AtsScore,
                a.CreatedAt
            })
            .ToListAsync();

        return Ok(history);
    }
}