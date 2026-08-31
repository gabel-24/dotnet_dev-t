using System.Net;
using System.Net.Http.Json;
using Xunit;
using JobApplicationApi.Dtos;

[Trait("Category", "Integration")]
public class CandidatesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CandidatesEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMe_ReturnsOk_WhenAuthenticated()
    {
        var token = await TestHelpers.RegisterAndLoginCandidateAsync(_client, "candme@test.com", "candme");
        TestHelpers.AttachToken(_client, token);

        var response = await _client.GetAsync("/api/candidates/me");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CandidateDto>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetMe_ReturnsUnauthorized_WhenNoToken()
    {
        var response = await _client.GetAsync("/api/candidates/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateMe_ReturnsNoContent_WhenSuccessful()
    {
        var token = await TestHelpers.RegisterAndLoginCandidateAsync(_client, "candupdate@test.com", "candupdate");
        TestHelpers.AttachToken(_client, token);

        var updateRequest = new UpdateCandidateDto
        {
            ResumeUrl = "http://example.com/new-resume.pdf",
            Skills = new List<string> { "React", "TypeScript" }
        };

        var response = await _client.PutAsJsonAsync("/api/candidates/me", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_ForNonexistentCandidate()
    {
        var response = await _client.GetAsync("/api/candidates/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}