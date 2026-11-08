using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using System.ComponentModel.DataAnnotations;

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
    public async Task<IActionResult> Signup(
        [FromBody] RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        if (!IsValidEmail(email))
            return BadRequest("Please provide a valid email address.");

        if (string.IsNullOrWhiteSpace(dto.Password) ||
            dto.Password.Length < 12 ||
            System.Text.Encoding.UTF8.GetByteCount(dto.Password) > 72)
        {
            return BadRequest(
                "Password must be at least 12 characters and no more than 72 UTF-8 bytes.");
        }

        var exists = await _db.Users
            .AnyAsync(u => u.Email == email);

        if (exists)
            return BadRequest("Email already exists.");

        var user = new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _db.Users.Add(user);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("An account with this email already exists.");
        }

        var token = _jwt.GenerateToken(user);

        return Ok(new { token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null ||
            !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid credentials.");
        }

        var token = _jwt.GenerateToken(user);

        return Ok(new { token });
    }

    private static bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) &&
               email.Length <= 254 &&
               new EmailAddressAttribute().IsValid(email);
    }
}
