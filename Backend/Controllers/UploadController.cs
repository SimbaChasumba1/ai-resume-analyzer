using Microsoft.AspNetCore.Mvc;
using backend.Services;
using System.Text;
using UglyToad.PdfPig;

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

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            string text = ExtractPdf(file);

            var result = await _openAI.AnalyzeResumeAsync(text);

            return Ok(new { success = true, analysis = result });
        }

        private string ExtractPdf(IFormFile file)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);

            StringBuilder sb = new();

            using var pdf = PdfDocument.Open(ms);
            foreach (var page in pdf.GetPages())
            {
                sb.AppendLine(page.Text);
            }

            return sb.ToString();
        }
    }
}

