using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(User user)
    {
        var key = _config["JWT_KEY"];
        var issuer = _config["JWT_ISSUER"];
        var audience = _config["JWT_AUDIENCE"];

        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException(
                "JWT_KEY is not configured.");

        if (string.IsNullOrWhiteSpace(issuer))
            throw new InvalidOperationException(
                "JWT_ISSUER is not configured.");

        if (string.IsNullOrWhiteSpace(audience))
            throw new InvalidOperationException(
                "JWT_AUDIENCE is not configured.");

        var keyBytes = Encoding.UTF8.GetBytes(key);

        if (keyBytes.Length < 32)
            throw new InvalidOperationException(
                "JWT_KEY must be at least 32 bytes.");

        var securityKey = new SymmetricSecurityKey(keyBytes);

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new Claim(
                ClaimTypes.Email,
                user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}