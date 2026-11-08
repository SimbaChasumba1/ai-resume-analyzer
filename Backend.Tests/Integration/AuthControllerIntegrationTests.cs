using System.Net;
using System.Text;
using System.Text.Json;
using backend.Data;
using backend.Models;
using Backend.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Tests.Integration;

public class AuthControllerIntegrationTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory = new();

    public async Task InitializeAsync()
    {
        await _factory.InitializeDatabaseAsync();
    }

    [Fact]
    public async Task Signup_ValidDetails_CreatesUserAndReturnsToken()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsync(
            "/auth/signup",
            JsonContent("  NewUser@Example.com  ", "StrongPassword123!"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);

        var token = document.RootElement
            .GetProperty("token")
            .GetString();

        Assert.False(string.IsNullOrWhiteSpace(token));

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await db.Users
            .SingleOrDefaultAsync(u => u.Email == "newuser@example.com");

        Assert.NotNull(user);
        Assert.NotEqual("StrongPassword123!", user.PasswordHash);
    }

    [Fact]
    public async Task Signup_DuplicateEmailWithDifferentCase_ReturnsBadRequest()
    {
        await SeedUserAsync(
            "existing@example.com",
            "ExistingPassword123!");

        using var client = _factory.CreateClient();

        var response = await client.PostAsync(
            "/auth/signup",
            JsonContent("EXISTING@EXAMPLE.COM", "AnotherPassword123!"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Signup_InvalidEmail_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsync(
            "/auth/signup",
            JsonContent("not-an-email", "StrongPassword123!"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Signup_WeakPassword_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsync(
            "/auth/signup",
            JsonContent("newuser@example.com", "short"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_EmailWithDifferentCase_ReturnsToken()
    {
        await SeedUserAsync(
            "loginuser@example.com",
            "CorrectPassword123!");

        using var client = _factory.CreateClient();

        var response = await client.PostAsync(
            "/auth/login",
            JsonContent("LOGINUSER@EXAMPLE.COM", "CorrectPassword123!"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);

        var token = document.RootElement
            .GetProperty("token")
            .GetString();

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    private async Task SeedUserAsync(string email, string password)
    {
        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        });

        await db.SaveChangesAsync();
    }

    private static StringContent JsonContent(string email, string password)
    {
        var json = JsonSerializer.Serialize(new
        {
            email,
            password
        });

        return new StringContent(
            json,
            Encoding.UTF8,
            "application/json");
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
    }
}