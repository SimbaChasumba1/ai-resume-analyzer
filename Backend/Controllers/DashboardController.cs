using backend.Data;

using backend.Models;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;



namespace backend.Controllers;



[ApiController]

[Route("api/dashboard")]

[Authorize]

public class DashboardController : ControllerBase

{

    private readonly AppDbContext _db;



    public DashboardController(AppDbContext db)

    {

        _db = db;

    }



    [HttpGet("history")]

    public IActionResult GetUserHistory()

    {

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdStr))

            return Unauthorized();



        // Parse the string to Guid

        if (!Guid.TryParse(userIdStr, out var userId))

            return Unauthorized();



        // Fetch uploads for this user

        var uploads = _db.ResumeUploads

            .Where(r => r.UserId == userId) // ✅ now Guid == Guid

            .OrderByDescending(r => r.CreatedAt)

            .Select(r => new

            {

                r.Id,

                r.FileName,

                r.CreatedAt,

                AnalysisResult = r.Analysis != null ? r.Analysis.Result : null

            })

            .ToList();



        return Ok(uploads);

    }

}

