[ApiController]
[Route("analysis")]
public class AIAnalysisController : ControllerBase
{
private readonly AppDbContext _db;


public AIAnalysisController(AppDbContext db)
{
_db = db;
}


[HttpGet("{resumeId}")]
public IActionResult Get(Guid resumeId)
{
var analysis = _db.AIAnalyses.FirstOrDefault(a => a.ResumeUploadId == resumeId);
if (analysis == null) return NotFound();
return Ok(analysis);
}
}