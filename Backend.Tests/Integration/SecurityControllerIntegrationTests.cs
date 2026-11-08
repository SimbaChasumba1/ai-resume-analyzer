using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using backend.Data;
using backend.Models;
using backend.Services;
using Backend.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Tests.Integration;

public class SecurityControllerIntegrationTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory = new();

    private HttpClient _ownerClient = null!;
    private HttpClient _otherClient = null!;

    private Guid _ownerAnalysisId;
    private Guid _otherAnalysisId;

    public async Task InitializeAsync()
    {
        await _factory.InitializeDatabaseAsync();

        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var owner = new User
        {
            Id = Guid.NewGuid(),
            Email = "owner@example.com",
            PasswordHash = "test-hash"
        };

        var otherUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "other@example.com",
            PasswordHash = "test-hash"
        };

        db.Users.AddRange(owner, otherUser);

        var ownerAnalysis = CreateAnalysis(owner);
        var otherAnalysis = CreateAnalysis(otherUser);

        db.ResumeUploads.AddRange(
            ownerAnalysis.Resume,
            otherAnalysis.Resume);

        db.AnalysisJobs.AddRange(
            ownerAnalysis.Job,
            otherAnalysis.Job);

        db.AIAnalyses.AddRange(
            ownerAnalysis.Analysis,
            otherAnalysis.Analysis);

        await db.SaveChangesAsync();

        _ownerAnalysisId = ownerAnalysis.Analysis.Id;
        _otherAnalysisId = otherAnalysis.Analysis.Id;

        var jwtService = scope.ServiceProvider
            .GetRequiredService<IJwtService>();

        var ownerToken = jwtService.GenerateToken(owner);
        var otherToken = jwtService.GenerateToken(otherUser);

        _ownerClient = CreateAuthenticatedClient(ownerToken);
        _otherClient = CreateAuthenticatedClient(otherToken);
    }

    [Fact]
    public async Task GetHistory_ReturnsOnlyCurrentUsersAnalyses()
    {
        var response = await _ownerClient.GetAsync("/history");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(body);

        var analyses = document.RootElement;

        Assert.Equal(1, analyses.GetArrayLength());

        var returnedId = analyses[0]
            .GetProperty("id")
            .GetGuid();

        Assert.Equal(_ownerAnalysisId, returnedId);
        Assert.NotEqual(_otherAnalysisId, returnedId);
    }

    [Fact]
    public async Task GetHistory_Unauthenticated_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/history");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task GetAnalysis_OtherUsersAnalysis_ReturnsNotFound()
    {
        var response = await _otherClient.GetAsync(
            $"/analysis/{_ownerAnalysisId}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task GetAnalysis_OwnAnalysis_ReturnsOk()
    {
        var response = await _ownerClient.GetAsync(
            $"/analysis/{_ownerAnalysisId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAnalysis_Unauthenticated_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(
            $"/analysis/{_ownerAnalysisId}");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task LegacyUpload_ReturnsNotFound()
    {
        using var content = new StringContent(
            "{}",
            Encoding.UTF8,
            "application/json");

        var response = await _ownerClient.PostAsync(
            "/upload",
            content);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    private HttpClient CreateAuthenticatedClient(string token)
    {
        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    private static (
        ResumeUpload Resume,
        AnalysisJob Job,
        AIAnalysis Analysis
    ) CreateAnalysis(User user)
    {
        var resume = new ResumeUpload
        {
            Id = Guid.NewGuid(),
            FileName = "resume.pdf",
            FilePath = "resume.pdf",
            ExtractedText = "Integration test resume",
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        var job = new AnalysisJob
        {
            Id = Guid.NewGuid(),
            ResumeUploadId = resume.Id,
            UserId = user.Id,
            Status = "Completed",
            RetryCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        var analysis = new AIAnalysis
        {
            Id = Guid.NewGuid(),
            AnalysisJobId = job.Id,
            ResumeFileName = resume.FileName,
            Summary = "Integration test analysis",
            StrengthsJson = "[]",
            WeaknessesJson = "[]",
            ImprovementsJson = "[]",
            AtsScore = 85,
            CreatedAt = DateTime.UtcNow
        };

        return (resume, job, analysis);
    }

    public async Task DisposeAsync()
    {
        _ownerClient.Dispose();
        _otherClient.Dispose();

        await _factory.DisposeAsync();
    }
}