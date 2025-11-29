using System.IO;
using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly OpenAIService _openAI;

        public UploadController(OpenAIService openAI)
        {
            _openAI = openAI;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> UploadAndAnalyze([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, file.FileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            // For now: simply convert file to text placeholder
            // You can integrate PDF or DOCX parsing later
            string extractedText = $"Resume saved at: {filePath}";

            var analysis = await _openAI.AnalyzeResumeAsync(extractedText);

            return Ok(new
            {
                success = true,
                fileName = file.FileName,
                analysis
            });
        }
    }
}

