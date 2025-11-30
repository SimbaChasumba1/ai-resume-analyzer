using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResumeController : ControllerBase
    {
        private readonly ResumeParser _parser;

        public ResumeController(ResumeParser parser)
        {
            _parser = parser;
        }

        [HttpPost("parse")]
        public IActionResult ParseResume([FromBody] string text)
        {
            var result = _parser.ParseText(text);
            return Ok(new { parsed = result });
        }
    }
}
