using System.Net.Http.Headers;

using System.Text;

using System.Text.Json;

using backend.Models;

using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.Logging;



namespace backend.Services;



public class PythonResumeAnalysisService : IResumeAnalysisService

{

    private readonly HttpClient _http;

    private readonly IConfiguration _config;

    private readonly ILogger<PythonResumeAnalysisService> _logger;



    public PythonResumeAnalysisService(

        HttpClient http,

        IConfiguration config,

        ILogger<PythonResumeAnalysisService> logger)

    {

        _http = http;

        _config = config;

        _logger = logger;

    }



    public async Task<ResumeAnalysisResult> AnalyzeAsync(string resumeText)

    {

        var baseUrl = _config["PythonService:BaseUrl"];



        var payload = new

        {

            text = resumeText

        };



        var json = new StringContent(

            JsonSerializer.Serialize(payload),

            Encoding.UTF8,

            "application/json"

        );



        HttpResponseMessage response;



        try

        {

            response = await _http.PostAsync(

                $"{baseUrl}/analyze/text",

                json

            );



            response.EnsureSuccessStatusCode();

        }

        catch (Exception ex)

        {

            _logger.LogError(ex, "Python service failed");

            throw new Exception("Python AI service unavailable.");

        }



        var result = await response.Content.ReadAsStringAsync();



        var options = new JsonSerializerOptions

        {

            PropertyNameCaseInsensitive = true

        };



        return JsonSerializer.Deserialize<ResumeAnalysisResult>(result, options)!;

    }

}



