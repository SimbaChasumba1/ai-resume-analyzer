using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using backend.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace backend.Services;

public class OpenAIResumeAnalysisService : IResumeAnalysisService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<OpenAIResumeAnalysisService> _logger;

    public OpenAIResumeAnalysisService(HttpClient http, IConfiguration config, ILogger<OpenAIResumeAnalysisService> logger)
    {
        _http = http;
        _config = config;
        _logger = logger;
    }

    public async Task<ResumeAnalysisResult> AnalyzeAsync(string resumeText)
    {
        var apiKey = _config["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
            throw new Exception("OpenAI API key is missing.");

        resumeText = resumeText.Length > 3000 ? resumeText.Substring(0, 3000) : resumeText;

        var prompt = $@"You are an expert recruiter.
Analyze the following resume and return STRICT JSON with:
- summary (string)
- strengths (array of strings)
- weaknesses (array of strings)
- atsScore (0-100 integer)
- improvements (array of strings)

Resume:
{resumeText}";

        var body = new
        {
            model = "gpt-4o-mini",
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.3
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        try
        {
            response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI request failed");
            throw new Exception("Failed to get AI analysis from OpenAI.");
        }

        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var content = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

        if (string.IsNullOrWhiteSpace(content))
            throw new Exception("OpenAI returned empty analysis.");

        return JsonSerializer.Deserialize<ResumeAnalysisResult>(content!)!;
    }
}
