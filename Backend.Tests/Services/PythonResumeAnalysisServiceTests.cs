using System.Net;
using System.Text;
using backend.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Backend.Tests.Services;

public class PythonResumeAnalysisServiceTests
{
    private readonly Mock<ILogger<PythonResumeAnalysisService>> _logger = new();

    private static IConfiguration CreateConfiguration()
    {
        var settings = new Dictionary<string, string?>
        {
            ["PythonService:BaseUrl"] = "http://localhost:8000"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    [Fact]
    public async Task AnalyzeAsync_SuccessfulResponse_ReturnsAnalysisResult()
    {
        // Arrange
        var json = """
        {
            "resumeFileName": "resume.pdf",
            "atsScore": 85,
            "summary": "Experienced software developer.",
            "createdAt": "2026-11-06T10:00:00Z",
            "strengths": ["C#", "ASP.NET Core"],
            "weaknesses": ["Limited experience"],
            "improvements": ["Add measurable achievements"]
        }
        """;

        var handler = new FakeHttpMessageHandler(
            HttpStatusCode.OK,
            json
        );

        var httpClient = new HttpClient(handler);

        var service = new PythonResumeAnalysisService(
            httpClient,
            CreateConfiguration(),
            _logger.Object
        );

        // Act
        var result = await service.AnalyzeAsync("Software developer resume");

        // Assert
        Assert.Equal("resume.pdf", result.ResumeFileName);
        Assert.Equal(85, result.AtsScore);
        Assert.Equal("Experienced software developer.", result.Summary);
        Assert.Contains("C#", result.Strengths);
        Assert.Contains("Limited experience", result.Weaknesses);
        Assert.Contains("Add measurable achievements", result.Improvements);

        Assert.Equal("/analyze/text", handler.RequestUri?.AbsolutePath);
        Assert.Equal("POST", handler.Method);
    }

    [Fact]
    public async Task AnalyzeAsync_HttpFailure_ThrowsServiceUnavailableException()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(
            HttpStatusCode.InternalServerError,
            "Internal Server Error"
        );

        var service = new PythonResumeAnalysisService(
            new HttpClient(handler),
            CreateConfiguration(),
            _logger.Object
        );

        // Act
        var exception = await Assert.ThrowsAsync<Exception>(
            () => service.AnalyzeAsync("Resume text")
        );

        // Assert
        Assert.Equal("Python AI service unavailable.", exception.Message);
    }

    [Fact]
    public async Task AnalyzeAsync_SendsResumeTextInJsonBody()
    {
        // Arrange
        var json = """
        {
            "resumeFileName": "resume.pdf",
            "atsScore": 75,
            "summary": "Test summary",
            "createdAt": "2026-11-06T10:00:00Z",
            "strengths": [],
            "weaknesses": [],
            "improvements": []
        }
        """;

        var handler = new FakeHttpMessageHandler(
            HttpStatusCode.OK,
            json
        );

        var service = new PythonResumeAnalysisService(
            new HttpClient(handler),
            CreateConfiguration(),
            _logger.Object
        );

        // Act
        await service.AnalyzeAsync("My resume content");

        // Assert
        Assert.NotNull(handler.RequestBody);
        Assert.Contains("My resume content", handler.RequestBody);
        Assert.Contains("\"text\"", handler.RequestBody);
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _responseContent;

        public Uri? RequestUri { get; private set; }
        public string? Method { get; private set; }
        public string? RequestBody { get; private set; }

        public FakeHttpMessageHandler(
            HttpStatusCode statusCode,
            string responseContent)
        {
            _statusCode = statusCode;
            _responseContent = responseContent;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            Method = request.Method.Method;

            RequestBody = request.Content == null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(
                    _responseContent,
                    Encoding.UTF8,
                    "application/json"
                )
            };
        }
    }
}