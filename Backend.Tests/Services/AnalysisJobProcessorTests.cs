using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Backend.Tests.Services;

public class AnalysisJobProcessorTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;
    private readonly Mock<IResumeAnalysisService> _analysisService;
    private readonly Mock<ILogger<AnalysisJobProcessor>> _logger;
    private readonly AnalysisJobProcessor _processor;

    public AnalysisJobProcessorTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();

        _analysisService = new Mock<IResumeAnalysisService>();
        _logger = new Mock<ILogger<AnalysisJobProcessor>>();

        _processor = new AnalysisJobProcessor(
            _db,
            _analysisService.Object,
            _logger.Object);
    }

    private async Task<(User user, ResumeUpload upload, AnalysisJob job)>
        CreateJobAsync(
            string extractedText = "Experienced C# developer with ASP.NET Core skills.",
            string status = "Pending")
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = "test-hash"
        };

        var upload = new ResumeUpload
        {
            Id = Guid.NewGuid(),
            FileName = "resume.pdf",
            FilePath = "/test/resume.pdf",
            ExtractedText = extractedText,
            UserId = user.Id,
            User = user
        };

        var job = new AnalysisJob
        {
            Id = Guid.NewGuid(),
            ResumeUploadId = upload.Id,
            ResumeUpload = upload,
            UserId = user.Id,
            User = user,
            Status = status
        };

        _db.Users.Add(user);
        _db.ResumeUploads.Add(upload);
        _db.AnalysisJobs.Add(job);

        await _db.SaveChangesAsync();

        return (user, upload, job);
    }

    private static ResumeAnalysisResult CreateAnalysisResult()
    {
        return new ResumeAnalysisResult
        {
            ResumeFileName = "resume.pdf",
            AtsScore = 85,
            Summary = "Experienced backend developer.",
            CreatedAt = DateTime.UtcNow,
            Strengths = new List<string>
            {
                "C#",
                "ASP.NET Core"
            },
            Weaknesses = new List<string>
            {
                "Limited cloud experience"
            },
            Improvements = new List<string>
            {
                "Add Azure projects"
            }
        };
    }

    [Fact]
    public async Task ProcessJobAsync_ValidJob_CompletesAndPersistsAnalysis()
    {
        // Arrange
        var (_, _, job) = await CreateJobAsync();

        var result = CreateAnalysisResult();

        _analysisService
            .Setup(x => x.AnalyzeAsync(It.IsAny<string>()))
            .ReturnsAsync(result);

        // Act
        var shouldRetry = await _processor.ProcessJobAsync(
            job.Id,
            CancellationToken.None);

        // Assert
        Assert.False(shouldRetry);

        var savedJob = await _db.AnalysisJobs
            .Include(j => j.Analysis)
            .FirstAsync(j => j.Id == job.Id);

        Assert.Equal("Completed", savedJob.Status);
        Assert.Equal(0, savedJob.RetryCount);
        Assert.NotNull(savedJob.StartedAt);
        Assert.NotNull(savedJob.CompletedAt);
        Assert.Null(savedJob.ErrorMessage);

        Assert.NotNull(savedJob.Analysis);
        Assert.Equal(85, savedJob.Analysis.AtsScore);
        Assert.Equal("resume.pdf", savedJob.Analysis.ResumeFileName);
        Assert.Equal(
            "Experienced backend developer.",
            savedJob.Analysis.Summary);

        Assert.Equal(
            new[] { "C#", "ASP.NET Core" },
            System.Text.Json.JsonSerializer.Deserialize<string[]>(
                savedJob.Analysis.StrengthsJson));

        _analysisService.Verify(
            x => x.AnalyzeAsync(
                "Experienced C# developer with ASP.NET Core skills."),
            Times.Once);
    }

    [Fact]
    public async Task ProcessJobAsync_MissingJob_DoesNotRetry()
    {
        // Arrange
        var missingJobId = Guid.NewGuid();

        // Act
        var shouldRetry = await _processor.ProcessJobAsync(
            missingJobId,
            CancellationToken.None);

        // Assert
        Assert.False(shouldRetry);

        _analysisService.Verify(
            x => x.AnalyzeAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessJobAsync_CompletedJob_DoesNotAnalyseAgain()
    {
        // Arrange
        var (_, _, job) = await CreateJobAsync(status: "Completed");

        // Act
        var shouldRetry = await _processor.ProcessJobAsync(
            job.Id,
            CancellationToken.None);

        // Assert
        Assert.False(shouldRetry);

        _analysisService.Verify(
            x => x.AnalyzeAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessJobAsync_FailedJob_DoesNotAnalyseAgain()
    {
        // Arrange
        var (_, _, job) = await CreateJobAsync(status: "Failed");

        // Act
        var shouldRetry = await _processor.ProcessJobAsync(
            job.Id,
            CancellationToken.None);

        // Assert
        Assert.False(shouldRetry);

        _analysisService.Verify(
            x => x.AnalyzeAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessJobAsync_ExistingAnalysis_MarksJobCompleted()
    {
        // Arrange
        var (_, _, job) = await CreateJobAsync();

        var existingAnalysis = new AIAnalysis
        {
            Id = Guid.NewGuid(),
            AnalysisJobId = job.Id,
            ResumeFileName = "resume.pdf",
            Summary = "Previously saved analysis.",
            AtsScore = 80,
            StrengthsJson = "[]",
            WeaknessesJson = "[]",
            ImprovementsJson = "[]",
            CreatedAt = DateTime.UtcNow
        };

        _db.AIAnalyses.Add(existingAnalysis);
        await _db.SaveChangesAsync();

        // Act
        var shouldRetry = await _processor.ProcessJobAsync(
            job.Id,
            CancellationToken.None);

        // Assert
        Assert.False(shouldRetry);

        var savedJob = await _db.AnalysisJobs
            .FirstAsync(j => j.Id == job.Id);

        Assert.Equal("Completed", savedJob.Status);
        Assert.NotNull(savedJob.CompletedAt);

        _analysisService.Verify(
            x => x.AnalyzeAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessJobAsync_EmptyResumeText_IncrementsRetryCount()
    {
        // Arrange
        var (_, _, job) = await CreateJobAsync(
            extractedText: "   ");

        // Act
        var shouldRetry = await _processor.ProcessJobAsync(
            job.Id,
            CancellationToken.None);

        // Assert
        Assert.True(shouldRetry);

        var savedJob = await _db.AnalysisJobs
            .FirstAsync(j => j.Id == job.Id);

        Assert.Equal(1, savedJob.RetryCount);
        Assert.Equal("Pending", savedJob.Status);

        Assert.Equal(
            "The uploaded resume contains no extracted text.",
            savedJob.ErrorMessage);

        _analysisService.Verify(
            x => x.AnalyzeAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessJobAsync_AnalysisServiceFails_IncrementsRetryCount()
    {
        // Arrange
        var (_, _, job) = await CreateJobAsync();

        _analysisService
            .Setup(x => x.AnalyzeAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("AI service unavailable."));

        // Act
        var shouldRetry = await _processor.ProcessJobAsync(
            job.Id,
            CancellationToken.None);

        // Assert
        Assert.True(shouldRetry);

        var savedJob = await _db.AnalysisJobs
            .FirstAsync(j => j.Id == job.Id);

        Assert.Equal(1, savedJob.RetryCount);
        Assert.Equal("Pending", savedJob.Status);
        Assert.Equal(
            "AI service unavailable.",
            savedJob.ErrorMessage);
    }

    [Fact]
    public async Task ProcessJobAsync_ThirdFailure_MarksJobFailed()
    {
        // Arrange
        var (_, _, job) = await CreateJobAsync();

        job.RetryCount = 2;
        await _db.SaveChangesAsync();

        _analysisService
            .Setup(x => x.AnalyzeAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("AI service unavailable."));

        // Act
        var shouldRetry = await _processor.ProcessJobAsync(
            job.Id,
            CancellationToken.None);

        // Assert
        Assert.False(shouldRetry);

        var savedJob = await _db.AnalysisJobs
            .FirstAsync(j => j.Id == job.Id);

        Assert.Equal(3, savedJob.RetryCount);
        Assert.Equal("Failed", savedJob.Status);
        Assert.Equal(
            "AI service unavailable.",
            savedJob.ErrorMessage);

        Assert.NotNull(savedJob.CompletedAt);
    }
[Fact]
public async Task ProcessJobAsync_SecondFailure_RemainsPending()
{
    // Arrange
    var (_, _, job) = await CreateJobAsync();

    job.RetryCount = 1;
    await _db.SaveChangesAsync();

    _analysisService
        .Setup(x => x.AnalyzeAsync(It.IsAny<string>()))
        .ThrowsAsync(new Exception("AI service unavailable."));

    // Act
    var shouldRetry = await _processor.ProcessJobAsync(
        job.Id,
        CancellationToken.None);

    // Assert
    Assert.True(shouldRetry);

    var savedJob = await _db.AnalysisJobs
        .FirstAsync(j => j.Id == job.Id);

    Assert.Equal(2, savedJob.RetryCount);
    Assert.Equal("Pending", savedJob.Status);
    Assert.Null(savedJob.CompletedAt);
}

[Fact]
public async Task ProcessJobAsync_Cancellation_DoesNotCountAsFailure()
{
    // Arrange
    var (_, _, job) = await CreateJobAsync();

    using var cancellationTokenSource =
        new CancellationTokenSource();

    cancellationTokenSource.Cancel();

    // Act
    await Assert.ThrowsAnyAsync<OperationCanceledException>(
        () => _processor.ProcessJobAsync(
            job.Id,
            cancellationTokenSource.Token));

    // Assert
    var savedJob = await _db.AnalysisJobs
        .FirstAsync(j => j.Id == job.Id);

    Assert.Equal(0, savedJob.RetryCount);
    Assert.Equal("Pending", savedJob.Status);
    Assert.Null(savedJob.ErrorMessage);

    _analysisService.Verify(
        x => x.AnalyzeAsync(It.IsAny<string>()),
        Times.Never);
}
    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}