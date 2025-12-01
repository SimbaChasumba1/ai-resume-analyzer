using backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Load OpenAI key
string? apiKey = builder.Configuration["OpenAI:ApiKey"];

// Register services
builder.Services.AddSingleton(new OpenAIService(apiKey!));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// FIX CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowFrontend");

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
