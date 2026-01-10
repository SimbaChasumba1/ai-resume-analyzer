using backend.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// ===== DATABASE CONFIG =====
// Get connection string from environment variable
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(databaseUrl))
{
    throw new InvalidOperationException("DATABASE_URL environment variable not set.");
}

// Convert Render DATABASE_URL to Npgsql connection string format
var databaseUri = new Uri(databaseUrl);
var userInfo = databaseUri.UserInfo.Split(':');

var connBuilder = new NpgsqlConnectionStringBuilder
{
    Host = databaseUri.Host,
    Port = databaseUri.Port > 0 ? databaseUri.Port : 5432,
    Username = userInfo[0],
    Password = userInfo[1],
    Database = databaseUri.AbsolutePath.TrimStart('/'),
    SslMode = SslMode.Require,
    TrustServerCertificate = true
};

// ===== SERVICES =====
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connBuilder.ConnectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// ===== APP =====
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();
