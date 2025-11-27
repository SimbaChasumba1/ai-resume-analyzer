using Microsoft.AspNetCore.Mvc;
using Backend.Services;

namespace Backend.Controllers
{
    [ApiController]
    [Route("ai")]
    public class AIAnalysisController : ControllerBase
    {
        private readonly PDFTextExtractor _pdf;
        private readonly OpenAIService _openai;

        public AIAnalysisController(PDFTextExtractor pdf, OpenAIService openai)
        {
            _pdf = pdf;
            _openai = openai;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File missing.");

            // 1. Extract text from PDF
            string rawText = await _pdf.ExtractTextAsync(file);

            if (string.IsNullOrWhiteSpace(rawText))
                return BadRequest("Could not extract text.");

            // 2. Send to OpenAI
            string aiResponse = await _openai.AnalyzeResume(rawText);

            return Ok(new
            {
                text = rawText,
                analysis = aiResponse
            });
        }
    }
}
