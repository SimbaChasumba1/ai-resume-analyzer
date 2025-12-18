using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Data;

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

    [HttpGet("{id}")]
    public IActionResult GetAnalysis(int id)
    {
        var analysis = _db.AIAnalyses.Find(id);
        if (analysis == null) return NotFound();
        return Ok(analysis);
    }
}
