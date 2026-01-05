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

        var key = _config["Jwt:Key"];

        var issuer = _config["Jwt:Issuer"];

        var audience = _config["Jwt:Audience"];



        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);



        var claims = new[]

        {

            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

            new Claim(ClaimTypes.Email, user.Email)

        };



        var token = new JwtSecurityToken(

            issuer,

            audience,

            claims,

            expires: DateTime.UtcNow.AddDays(7),

            signingCredentials: credentials

        );



        return new JwtSecurityTokenHandler().WriteToken(token);

    }

}