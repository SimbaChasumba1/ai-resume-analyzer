using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AIAnalysisController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AIAnalysisController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeResume([FromBody] ResumeAnalysisRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ResumeText))
                return BadRequest("Resume text is required.");

            // Fake placeholder analysis (replace with your AI)
            string analysis = $"Analysis for: {request.FileName}";

            var saved = new ResumeAnalysis
            {
                ResumeText = request.ResumeText,
                AnalysisResult = analysis
            };

            _db.ResumeAnalyses.Add(saved);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Analysis complete", analysis });
        }
    }
}
