[Authorize]
[HttpPost("analyze")]
public async Task<IActionResult> Analyze([FromForm] IFormFile file)
{
    if (file == null) return BadRequest("No file uploaded");

    // Tomorrow: AI processing here
    return Ok(new { message = "Analysis ready" });
}
