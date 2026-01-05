using System.Net.Http.Headers;

using System.Text;

using System.Text.Json;

using Microsoft.Extensions.Configuration;

using backend.Models;



namespace backend.Services;



public interface IResumeAnalysisService

{

    Task<ResumeAnalysisResult> AnalyzeAsync(string resumeText);

}



public class ResumeAnalysisService : IResumeAnalysisService

{

    private readonly HttpClient _http;

    private readonly IConfiguration _config;



    public ResumeAnalysisService(HttpClient http, IConfiguration config)

    {

        _http = http;

        _config = config;

    }



    public async Task<ResumeAnalysisResult> AnalyzeAsync(string resumeText)

    {

        var apiKey = _config["OpenAI:ApiKey"];



        var prompt = $"""

        You are an expert technical recruiter.



        Analyze the following resume and return STRICT JSON with:

        - summary

        - strengths (array)

        - weaknesses (array)

        - atsScore (0-100)

        - improvementSuggestions (array)



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



        var request = new HttpRequestMessage(

            HttpMethod.Post,

            "https://api.openai.com/v1/chat/completions"

        );



        request.Headers.Authorization =

            new AuthenticationHeaderValue("Bearer", apiKey);



        request.Content = new StringContent(

            JsonSerializer.Serialize(body),

            Encoding.UTF8,

            "application/json"

        );



        var response = await _http.SendAsync(request);

        response.EnsureSuccessStatusCode();



        var json = await response.Content.ReadAsStringAsync();



        using var doc = JsonDocument.Parse(json);

        var content = doc.RootElement

            .GetProperty("choices")[0]

            .GetProperty("message")

            .GetProperty("content")

            .GetString();



        return JsonSerializer.Deserialize<ResumeAnalysisResult>(content!)!;

    }

}

