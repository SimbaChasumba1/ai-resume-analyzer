using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Backend.Tests.Infrastructure;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection =
        new("Data Source=:memory:");

    public CustomWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable(
            "DB_HOST", "localhost");

        Environment.SetEnvironmentVariable(
            "DB_PORT", "5432");

        Environment.SetEnvironmentVariable(
            "DB_NAME", "test");

        Environment.SetEnvironmentVariable(
            "DB_USER", "test");

        Environment.SetEnvironmentVariable(
            "DB_PASSWORD", "test");

        Environment.SetEnvironmentVariable(
            "JWT_KEY",
            "integration-test-secret-key-at-least-32-bytes");

        Environment.SetEnvironmentVariable(
            "JWT_ISSUER", "AIResumeAnalyzer.Tests");

        Environment.SetEnvironmentVariable(
            "JWT_AUDIENCE", "AIResumeAnalyzer.Tests");

        _connection.Open();
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(_connection));

            services.RemoveAll<IHostedService>();

            services.RemoveAll<IPdfTextExtractor>();

            services.AddScoped<
                IPdfTextExtractor,
                FakePdfTextExtractor>();
        });
    }

    public async Task InitializeDatabaseAsync()
    {
        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        await db.Database.EnsureCreatedAsync();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
            _connection.Dispose();
    }
}