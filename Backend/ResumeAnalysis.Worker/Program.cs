using backend.Data;
using backend.Services;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using ResumeAnalysis.Worker;
using System.IO;

var envPath = Path.Combine(
    Directory.GetCurrentDirectory(),
    "..",
    ".env"
);

Env.Load(Path.GetFullPath(envPath));

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// PostgreSQL configuration
var host = Environment.GetEnvironmentVariable("DB_HOST")
    ?? throw new InvalidOperationException("DB_HOST is missing.");

var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";

var database = Environment.GetEnvironmentVariable("DB_NAME")
    ?? throw new InvalidOperationException("DB_NAME is missing.");

var username = Environment.GetEnvironmentVariable("DB_USER")
    ?? throw new InvalidOperationException("DB_USER is missing.");

var password = Environment.GetEnvironmentVariable("DB_PASSWORD")
    ?? throw new InvalidOperationException("DB_PASSWORD is missing.");

var connectionString =
    $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Existing Python AI service
builder.Services.AddHttpClient<
    IResumeAnalysisService,
    PythonResumeAnalysisService>();

// Analysis job processor
builder.Services.AddScoped<AnalysisJobProcessor>();

// Background worker
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

await app.RunAsync();