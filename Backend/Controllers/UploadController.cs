[ApiController]
[Route("upload")]
public class UploadController : ControllerBase
{
private readonly AppDbContext _db;


public UploadController(AppDbContext db)
{
_db = db;
}


[HttpPost]
public async Task<IActionResult> Upload(IFormFile file)
{
if (file == null || file.Length == 0)
return BadRequest("Invalid file");


var resume = new ResumeUpload
{
FileName = file.FileName,
UserId = Guid.Empty // replace with JWT user later
};


_db.ResumeUploads.Add(resume);
await _db.SaveChangesAsync();


return Ok(new { resume.Id });
}
}