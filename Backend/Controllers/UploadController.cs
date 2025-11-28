using Microsoft.AspNetCore.Mvc;
using Backend.Services;

namespace Backend.Controllers
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

        [HttpPost]
        [Route("analyze")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // SIMPLE TEXT (replace with PDF extraction later)
            string extractedText = "Extracted resume text placeholder.";

            var aiResult = await _openAI.AnalyzeResume(extractedText);

            return Ok(new
            {
                uploaded = true,
                file = file.FileName,
                aiAnalysis = aiResult
            });
        }
    }
}
