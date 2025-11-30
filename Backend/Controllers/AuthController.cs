using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            var token = _jwtService.GenerateToken("12345");
            return Ok(new { token });
        }
    }
}
