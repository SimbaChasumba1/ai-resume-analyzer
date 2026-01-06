using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using backend.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace backend.Services;

public interface IResumeAnalysisService
{
    Task<ResumeAnalysisResult> AnalyzeAsync(string resumeText);
}

public class ResumeAnalysisService : IResumeAnalysisService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<ResumeAnalysisService> _logger;

    public ResumeAnalysisService(HttpClient http, IConfiguration config, ILogger<ResumeAnalysisService> logger)
    {
        _http = http;
        _config = config;
        _logger = logger;
    }

    public async Task<ResumeAnalysisResult> AnalyzeAsync(string resumeText)
    {
        var apiKey = _config["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("OpenAI API key is missing in configuration.");
        }

        // Optional: limit resume text to prevent token overflow
        resumeText = resumeText.Length > 3000 ? resumeText.Substring(0, 3000) : resumeText;

        var prompt = $"""
        You are an expert technical recruiter.

        Analyze the following resume and return STRICT JSON with:

        - summary (string)
        - strengths (array of strings)
        - weaknesses (array of strings)
        - atsScore (0-100 integer)
        - improvements (array of strings)

        Resume:
        {resumeText}
        """;

        var body = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
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

        string? content;
        try
        {
            using var doc = JsonDocument.Parse(json);
            content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse OpenAI response JSON: {Json}", json);
            throw new Exception("Invalid response format from OpenAI.");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            _logger.LogError("OpenAI returned empty content: {Json}", json);
            throw new Exception("OpenAI returned empty analysis.");
        }

        ResumeAnalysisResult result;
        try
        {
            result = JsonSerializer.Deserialize<ResumeAnalysisResult>(content!)!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize AI response: {Content}", content);
            throw new Exception("AI returned invalid JSON format.");
        }

        // Ensure arrays are non-null
        result.Strengths ??= new List<string>();
        result.Weaknesses ??= new List<string>();
        result.Improvements ??= new List<string>();

        return result;
    }
}
