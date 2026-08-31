using System.Net;
using System.Net.Http.Json;
using Xunit;
using JobApplicationApi.Dtos;

[Trait("Category", "Integration")]
public class RecruitersEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RecruitersEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMe_ReturnsOk_WhenAuthenticated()
    {
        var token = await TestHelpers.RegisterAndLoginRecruiterAsync(_client, "recme@test.com", "recme");
        TestHelpers.AttachToken(_client, token);

        var response = await _client.GetAsync("/api/recruiters/me");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<RecruiterDto>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateMe_ReturnsNoContent_WhenSuccessful()
    {
        var token = await TestHelpers.RegisterAndLoginRecruiterAsync(_client, "recupdate@test.com", "recupdate");
        TestHelpers.AttachToken(_client, token);

        var updateRequest = new UpdateRecruiterDto
        {
            CompanyName = "NewCo",
            Department = "Engineering"
        };

        var response = await _client.PutAsJsonAsync("/api/recruiters/me", updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetMe_ReturnsForbidden_WhenCandidateTokenUsed()
    {
        var token = await TestHelpers.RegisterAndLoginCandidateAsync(_client, "notrecruiter@test.com", "notrecruiter");
        TestHelpers.AttachToken(_client, token);

        var response = await _client.GetAsync("/api/recruiters/me");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}