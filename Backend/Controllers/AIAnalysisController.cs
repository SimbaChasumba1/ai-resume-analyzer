using System.Security.Claims;
using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("analysis")]
[Authorize]
public class AIAnalysisController : ControllerBase
{
    private readonly AppDbContext _db;

    public AIAnalysisController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAnalysis(Guid id)
    {
        if (!Guid.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out var userId))
        {
            return Unauthorized();
        }

        var analysis = await _db.AIAnalyses
            .AsNoTracking()
            .Where(a =>
                a.Id == id &&
                a.AnalysisJob.UserId == userId)
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
            .FirstOrDefaultAsync();

        if (analysis == null)
            return NotFound();

        return Ok(analysis);
    }
}