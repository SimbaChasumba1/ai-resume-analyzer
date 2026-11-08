using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using backend.Data;
using backend.Models;
using backend.Services;
using Backend.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Tests.Integration;

public class ResumeControllerIntegrationTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory = new();

    private HttpClient _client = null!;
    private Guid _userId;

    public async Task InitializeAsync()
    {
        await _factory.InitializeDatabaseAsync();

        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "integration-test@example.com",
            PasswordHash = "test-password-hash"
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        _userId = user.Id;

        var jwtService = scope.ServiceProvider
            .GetRequiredService<IJwtService>();

        var token = jwtService.GenerateToken(user);

        _client = _factory.CreateClient();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task UploadResume_ValidPdf_CreatesAllThreeRecords()
    {
        // Arrange
        using var content = CreateUpload(
            "resume.pdf",
            [1, 2, 3]);

        // Act
        var response = await _client.PostAsync(
            "/api/resume/upload",
            content);

        // Assert
        Assert.Equal(
            HttpStatusCode.Accepted,
            response.StatusCode);

        var responseBody =
            await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(responseBody);

        var jobId = document.RootElement
            .GetProperty("jobId")
            .GetGuid();

        Assert.Equal(
            "Pending",
            document.RootElement.GetProperty("status").GetString());

        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var resume = await db.ResumeUploads.SingleAsync();

        var job = await db.AnalysisJobs.SingleAsync();

        var outbox = await db.OutboxMessages.SingleAsync();

        Assert.Equal("resume.pdf", resume.FileName);
        Assert.Equal(_userId, resume.UserId);

        Assert.Equal(resume.Id, job.ResumeUploadId);
        Assert.Equal(jobId, job.Id);
        Assert.Equal(_userId, job.UserId);
        Assert.Equal("Pending", job.Status);
        Assert.Equal(0, job.RetryCount);

        Assert.Equal("ResumeAnalysisRequested", outbox.Type);
        Assert.Null(outbox.ProcessedAt);
        Assert.Equal(0, outbox.Attempts);

        var message = JsonSerializer.Deserialize<AnalysisJobMessage>(
            outbox.Payload);

        Assert.NotNull(message);
        Assert.Equal(job.Id, message.JobId);
    }

    [Fact]
    public async Task UploadResume_InvalidExtension_ReturnsBadRequest()
    {
        // Arrange
        using var content = CreateUpload(
            "resume.txt",
            [1, 2, 3]);

        // Act
        var response = await _client.PostAsync(
            "/api/resume/upload",
            content);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);

        await AssertNoUploadRecordsAsync();
    }

    [Fact]
    public async Task UploadResume_EmptyFile_ReturnsBadRequest()
    {
        // Arrange
        using var content = CreateUpload(
            "resume.pdf",
            []);

        // Act
        var response = await _client.PostAsync(
            "/api/resume/upload",
            content);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);

        await AssertNoUploadRecordsAsync();
    }

    [Fact]
    public async Task UploadResume_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        using var unauthenticatedClient =
            _factory.CreateClient();

        using var content = CreateUpload(
            "resume.pdf",
            [1, 2, 3]);

        // Act
        var response = await unauthenticatedClient.PostAsync(
            "/api/resume/upload",
            content);

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);

        await AssertNoUploadRecordsAsync();
    }

    [Fact]
    public async Task GetJobStatus_OtherUsersJob_ReturnsNotFound()
    {
        // Arrange: create a job belonging to the first user.
        using var content = CreateUpload(
            "resume.pdf",
            [1, 2, 3]);

        var uploadResponse = await _client.PostAsync(
            "/api/resume/upload",
            content);

        Assert.Equal(
            HttpStatusCode.Accepted,
            uploadResponse.StatusCode);

        var responseBody =
            await uploadResponse.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(responseBody);

        var jobId = document.RootElement
            .GetProperty("jobId")
            .GetGuid();

        // Create another user.
        User otherUser;

        string otherToken;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            otherUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "another-user@example.com",
                PasswordHash = "test-password-hash"
            };

            db.Users.Add(otherUser);

            await db.SaveChangesAsync();

            var jwtService = scope.ServiceProvider
                .GetRequiredService<IJwtService>();

            otherToken = jwtService.GenerateToken(otherUser);
        }

        using var otherClient = _factory.CreateClient();

        otherClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", otherToken);

        // Act
        var response = await otherClient.GetAsync(
            $"/api/resume/analysis/{jobId}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    private static MultipartFormDataContent CreateUpload(
        string fileName,
        byte[] bytes)
    {
        var content = new MultipartFormDataContent();

        var fileContent = new ByteArrayContent(bytes);

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue("application/pdf");

        content.Add(fileContent, "file", fileName);

        return content;
    }

    private async Task AssertNoUploadRecordsAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        Assert.Equal(0, await db.ResumeUploads.CountAsync());
        Assert.Equal(0, await db.AnalysisJobs.CountAsync());
        Assert.Equal(0, await db.OutboxMessages.CountAsync());
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();

        await _factory.DisposeAsync();
    }
}