using JobApplicationApi.Dtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

[Trait("Category", "Integration")]
public class JobPostingsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public JobPostingsEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPaged_ReturnsOk_ForAnonymousUser()
    {
        var response = await _client.GetAsync("/api/jobpostings");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<JobPostingSummaryDto>>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenRecruiterAuthenticated()
    {
        var token = await TestHelpers.RegisterAndLoginRecruiterAsync(_client, "postcreate@test.com", "postcreate");
        TestHelpers.AttachToken(_client, token);

        var request = new CreateJobPostingDto
        {
            Title = "Backend Developer",
            Description = "Build APIs",
            Location = "Nairobi",
            EmploymentType = "Full-time"
        };

        var response = await _client.PostAsJsonAsync("/api/jobpostings", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<JobPostingDto>();
        Assert.Equal("Backend Developer", result!.Title);
    }

    [Fact]
    public async Task Create_ReturnsForbidden_WhenCandidateTriesToCreate()
    {
        var token = await TestHelpers.RegisterAndLoginCandidateAsync(_client, "candpost@test.com", "candpost");
        TestHelpers.AttachToken(_client, token);

        var request = new CreateJobPostingDto
        {
            Title = "Should Not Work",
            Description = "N/A"
        };

        var response = await _client.PostAsJsonAsync("/api/jobpostings", request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenAnotherRecruiterOwnsPosting()
    {
        var ownerToken = await TestHelpers.RegisterAndLoginRecruiterAsync(_client, "owner@test.com", "owner");
        TestHelpers.AttachToken(_client, ownerToken);

        var createResponse = await _client.PostAsJsonAsync("/api/jobpostings", new CreateJobPostingDto
        {
            Title = "Original Posting",
            Description = "Desc"
        });
        var posting = await createResponse.Content.ReadFromJsonAsync<JobPostingDto>();

        var otherToken = await TestHelpers.RegisterAndLoginRecruiterAsync(_client, "intruder@test.com", "intruder");
        TestHelpers.AttachToken(_client, otherToken);

        var updateResponse = await _client.PutAsJsonAsync($"/api/jobpostings/{posting!.Id}", new UpdateJobPostingDto
        {
            Title = "Hijacked Title"
        });

        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
    }
}