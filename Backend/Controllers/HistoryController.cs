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
public IActionResult Get()
{
var userId = int.Parse(User.FindFirst("id")!.Value);


var uploads = _db.ResumeUploads
.Where(r => r.UserId == userId)
.Include(r => r.Analysis)
.OrderByDescending(r => r.CreatedAt)
.ToList();


return Ok(uploads);
}
}