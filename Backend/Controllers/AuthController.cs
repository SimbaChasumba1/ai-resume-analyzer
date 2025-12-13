[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
private readonly AppDbContext _db;
private readonly IJwtService _jwt;


public AuthController(AppDbContext db, IJwtService jwt)
{
_db = db;
_jwt = jwt;
}


[HttpPost("register")]
public async Task<IActionResult> Register(RegisterDto dto)
{
var user = new User
{
FullName = dto.FullName,
Email = dto.Email,
PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
};


_db.Users.Add(user);
await _db.SaveChangesAsync();


return Ok();
}


[HttpPost("login")]
public IActionResult Login(LoginDto dto)
{
var user = _db.Users.SingleOrDefault(u => u.Email == dto.Email);
if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
return Unauthorized();


var token = _jwt.Generate(user);
return Ok(new { token });
}
}