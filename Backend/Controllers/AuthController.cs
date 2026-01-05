using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using backend.Data;

using backend.DTOs;

using backend.Models;

using backend.Services;

using BCrypt.Net;



namespace backend.Controllers;



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



    [HttpPost("signup")]

    public async Task<IActionResult> Signup([FromBody] RegisterDto dto)

    {

        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))

            return BadRequest("Email already exists");



        var user = new User

        {

            Email = dto.Email,

            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)

        };



        _db.Users.Add(user);

        await _db.SaveChangesAsync();



        var token = _jwt.GenerateToken(user);

        return Ok(new { token });

    }



    [HttpPost("login")]

    public async Task<IActionResult> Login([FromBody] LoginDto dto)

    {

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null) return Unauthorized("Invalid credentials");



        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))

            return Unauthorized("Invalid credentials");



        var token = _jwt.GenerateToken(user);

        return Ok(new { token });

    }

}

