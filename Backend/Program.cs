using backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new OpenAIService(
    builder.Configuration["OPENAI_API_KEY"]
));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
{
    p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
}));

var app = builder.Build();
app.UseCors();
app.MapControllers();
app.Run();

