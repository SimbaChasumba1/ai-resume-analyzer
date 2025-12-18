using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("history")]
    public class HistoryController : ControllerBase
    {
        private readonly AppDbContext _db;

        public HistoryController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetHistory()
        {
            return Ok(_db.AIAnalyses.ToList());
        }
    }
}
