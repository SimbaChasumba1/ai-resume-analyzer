using System.Text;
using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);



// Database configuration

var host = builder.Configuration["DB_HOST"]
    ?? throw new InvalidOperationException("DB_HOST is missing.");

var port = builder.Configuration["DB_PORT"] ?? "5432";

var database = builder.Configuration["DB_NAME"]
    ?? throw new InvalidOperationException("DB_NAME is missing.");

var username = builder.Configuration["DB_USER"]
    ?? throw new InvalidOperationException("DB_USER is missing.");

var password = builder.Configuration["DB_PASSWORD"]
    ?? throw new InvalidOperationException("DB_PASSWORD is missing.");

var connectionString =
    $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;Channel Binding=Disable";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));


// Controllers

builder.Services.AddControllers();

// JWT configuration

builder.Services.AddScoped<IJwtService, JwtService>();

var jwtKey = builder.Configuration["JWT_KEY"]
    ?? throw new InvalidOperationException("JWT_KEY is missing.");

var jwtIssuer = builder.Configuration["JWT_ISSUER"]
    ?? throw new InvalidOperationException("JWT_ISSUER is missing.");

var jwtAudience = builder.Configuration["JWT_AUDIENCE"]
    ?? throw new InvalidOperationException("JWT_AUDIENCE is missing.");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "JWT_KEY must be at least 32 bytes.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
            };
    });

builder.Services.AddAuthorization();


// CORS

var frontendOrigins = new[]
{
    "http://localhost:3000",
    "https://ai-resume-analysis-platform.vercel.app",
    builder.Configuration["FRONTEND_ORIGIN"]
}
.Where(origin => !string.IsNullOrWhiteSpace(origin))
.Distinct()
.ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(frontendOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

// Existing application services

builder.Services.AddHttpClient<IResumeAnalysisService, PythonResumeAnalysisService>();
builder.Services.AddScoped<IPdfTextExtractor, PdfTextExtractor>();
builder.Services.AddHostedService<RabbitMqOutboxPublisher>();


builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");

// Middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "AI Resume Analyzer API v1"));
}

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }