using Microsoft.AspNetCore.Mvc;
using backend.Services;
using System.IO;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly OpenAIService _openAI;
        private readonly ResumeParser _parser;

        public UploadController(OpenAIService openAI, ResumeParser parser)
        {
            _openAI = openAI;
            _parser = parser;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, Path.GetFileName(file.FileName));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Extract text
            var extractedText = _parser.ParseText(filePath);

            // Call OpenAI service
            var analysis = await _openAI.AnalyzeResumeAsync(extractedText);

            return Ok(new { uploaded = true, fileName = file.FileName, analysis });
        }
    }
}
