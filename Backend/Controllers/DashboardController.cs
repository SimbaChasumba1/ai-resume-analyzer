using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
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

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return Unauthorized();

            // Fetching the analyses for the user
            var analyses = await _db.AIAnalyses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(); // Retrieve as a list first, without deserialization

            // Now deserialize the JSON fields in-memory (after fetching the data)
            var result = analyses.Select(a => new ResumeAnalysisResult
            {
                ResumeFileName = a.ResumeFileName,
                AtsScore = a.AtsScore,
                Summary = a.Summary,
                CreatedAt = a.CreatedAt,
                Strengths = JsonSerializer.Deserialize<List<string>>(a.StrengthsJson) ?? new List<string>(),
                Weaknesses = JsonSerializer.Deserialize<List<string>>(a.WeaknessesJson) ?? new List<string>(),
                Improvements = JsonSerializer.Deserialize<List<string>>(a.ImprovementsJson) ?? new List<string>()
            }).ToList();

            return Ok(new
            {
                user = new
                {
                    user.Email,
                    user.CreatedAt
                },
                uploads = result // List of ResumeAnalysisResult objects
            });
        }
    }
}
