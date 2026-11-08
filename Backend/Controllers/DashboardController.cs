
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Collections.Generic;
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
            if (!Guid.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
            {
                return Unauthorized();
            }

            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return Unauthorized();

            var jobs = await _db.AnalysisJobs
                .AsNoTracking()
                .Include(j => j.ResumeUpload)
                .Include(j => j.Analysis)
                .Where(j => j.UserId == userId)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            var uploads = jobs.Select(j => new
            {
                JobId = j.Id,
                ResumeFileName = j.ResumeUpload.FileName,
                Status = j.Status,
                CreatedAt = j.CreatedAt,
                CompletedAt = j.CompletedAt,

                Analysis = j.Analysis == null ? null : new
                {
                    j.Analysis.Id,
                    j.Analysis.ResumeFileName,
                    j.Analysis.AtsScore,
                    j.Analysis.Summary,

                    Strengths =
                        JsonSerializer.Deserialize<List<string>>(
                            j.Analysis.StrengthsJson
                        ) ?? new List<string>(),

                    Weaknesses =
                        JsonSerializer.Deserialize<List<string>>(
                            j.Analysis.WeaknessesJson
                        ) ?? new List<string>(),

                    Improvements =
                        JsonSerializer.Deserialize<List<string>>(
                            j.Analysis.ImprovementsJson
                        ) ?? new List<string>()
                }
            }).ToList();

            return Ok(new
            {
                user = new
                {
                    user.Email,
                    user.CreatedAt
                },
                uploads
            });
        }
    }
}
