using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("upload")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly AppDbContext _db;

    public UploadController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public IActionResult UploadResume([FromBody] ResumeUpload upload)
    {
        _db.ResumeUploads.Add(upload);
        _db.SaveChanges();
        return Ok(upload);
    }
}
