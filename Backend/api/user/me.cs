using backend.Data;

using backend.DTOs;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;



namespace backend.Controllers

{

    [ApiController]

    [Route("api/user")]

    [Authorize]

    public class UserController : ControllerBase

    {

        private readonly AppDbContext _db;



        public UserController(AppDbContext db)

        {

            _db = db;

        }



        [HttpGet("me")]

        public IActionResult GetMe()

        {

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdClaim == null) return Unauthorized();



            var user = _db.Users.FirstOrDefault(u => u.Id.ToString() == userIdClaim);

            if (user == null) return NotFound();



            return Ok(new MeDto

            {

                Email = user.Email

            });

        }

    }

}
