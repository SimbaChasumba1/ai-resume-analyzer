using backend.Data;
using backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add OpenAIService as singleton
builder.Services.AddSingleton(new OpenAIService(
    builder.Configuration["OPENAI_API_KEY"]!
));

// Add EF Core with Npgsql
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add Controllers
builder.Services.AddControllers();

// Add Swagger (optional)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin();
    });
});

var app = builder.Build();

// Use Swagger (optional)
app.UseSwagger();
app.UseSwaggerUI();

// Enable CORS
app.UseCors();

// Use HTTPS redirection
app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

app.Run();
