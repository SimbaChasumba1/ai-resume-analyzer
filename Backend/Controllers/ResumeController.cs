using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ResumesController : ControllerBase
{
    private readonly ResumeParserService _resumeParserService;

    public ResumesController(ResumeParserService resumeParserService)
    {
        _resumeParserService = resumeParserService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        if (!file.FileName.EndsWith(".pdf") && !file.FileName.EndsWith(".docx"))
            return BadRequest("Invalid file format. Only PDF and DOCX are supported.");

        if (file.Length > 5 * 1024 * 1024) // 5MB size limit
            return BadRequest("File is too large. Max size is 5MB.");

        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        if (!Directory.Exists(uploads))
            Directory.CreateDirectory(uploads);

        var filePath = Path.Combine(uploads, file.FileName);

        using (var stream = System.IO.File.Create(filePath))
            await file.CopyToAsync(stream);

        // Parse the resume and extract skills
        var skills = _resumeParserService.ExtractSkillsFromResume(filePath);

        return Ok(new { id = 1, skills = skills });
    }
}
