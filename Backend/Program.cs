using System.Text;
using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// DB
string? databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Convert Render DATABASE_URL to Npgsql format
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Controllers
builder.Services.AddControllers();

// JWT
builder.Services.AddScoped<IJwtService, JwtService>();

var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("https://ai-resume-analysis-platform.vercel.app") // ✅ Frontend URL
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IResumeAnalysisService, OpenAIResumeAnalysisService>();
builder.Services.AddScoped<IPdfTextExtractor, PdfTextExtractor>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Resume Analyzer API v1"));

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
