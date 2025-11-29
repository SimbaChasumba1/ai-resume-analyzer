using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIAnalysisController : ControllerBase
    {
        private readonly OpenAIService _openAI;

        public AIAnalysisController(OpenAIService openAI)
        {
            _openAI = openAI;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze([FromBody] ResumeAnalysisRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ResumeText))
                return BadRequest("ResumeText is required.");

            var analysis = await _openAI.AnalyzeResumeAsync(request.ResumeText);
            return Ok(new { success = true, analysis });
        }
    }
}
