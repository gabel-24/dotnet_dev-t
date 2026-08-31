using Microsoft.AspNetCore.Identity.Data;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using JobApplicationApi.Dtos;

[Trait("Category", "Integration")]
public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegisterCandidate_ReturnsOk_AndToken()
    {
        var request = new RegisterCandidateDto
        {
            Email = "candidate1@test.com",
            Password = "Password123",
            Username = "candidate1",
            ResumeUrl = "http://example.com/resume.pdf",
            Skills = new List<string> { "C#", "SQL" }
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register/candidate", request);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AuthDto>();

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result!.Token));
        Assert.Equal("Candidate", result.Role);
    }

    [Fact]
    public async Task RegisterCandidate_ReturnsBadRequest_WhenEmailAlreadyExists()
    {
        var request = new RegisterCandidateDto
        {
            Email = "duplicate@test.com",
            Password = "Password123",
            Username = "dupuser",
            Skills = new List<string>()
        };

        await _client.PostAsJsonAsync("/api/auth/register/candidate", request);
        var secondResponse = await _client.PostAsJsonAsync("/api/auth/register/candidate", request);

        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsToken_WithValidCredentials()
    {
        var registerRequest = new RegisterCandidateDto
        {
            Email = "logintest@test.com",
            Password = "Password123",
            Username = "loginuser",
            Skills = new List<string>()
        };
        await _client.PostAsJsonAsync("/api/auth/register/candidate", registerRequest);

        var loginRequest = new LoginRequest
        {
            Email = "logintest@test.com",
            Password = "Password123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AuthDto>();

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result!.Token));
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WithWrongPassword()
    {
        var registerRequest = new RegisterCandidateDto
        {
            Email = "wrongpass@test.com",
            Password = "Password123",
            Username = "wrongpassuser",
            Skills = new List<string>()
        };
        await _client.PostAsJsonAsync("/api/auth/register/candidate", registerRequest);

        var loginRequest = new LoginRequest
        {
            Email = "wrongpass@test.com",
            Password = "WrongPassword"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}