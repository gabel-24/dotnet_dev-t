using JobApplicationApi.Dtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

[Trait("Category", "Integration")]
public class InterviewStagesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public InterviewStagesEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<(string recruiterToken, JobApplicationDto application)> SetupApplicationAsync(string suffix)
    {
        var recruiterToken = await TestHelpers.RegisterAndLoginRecruiterAsync(_client, $"isrec{suffix}@test.com", $"isrec{suffix}");
        TestHelpers.AttachToken(_client, recruiterToken);

        var postingResponse = await _client.PostAsJsonAsync("/api/jobpostings", new CreateJobPostingDto
        {
            Title = "IS Test Posting",
            Description = "Desc"
        });
        postingResponse.EnsureSuccessStatusCode();
        var posting = await postingResponse.Content.ReadFromJsonAsync<JobPostingDto>();

        var candidateToken = await TestHelpers.RegisterAndLoginCandidateAsync(_client, $"iscand{suffix}@test.com", $"iscand{suffix}");
        TestHelpers.AttachToken(_client, candidateToken);

        var applyResponse = await _client.PostAsJsonAsync(
            $"/api/jobpostings/{posting!.Id}/applications",
            new CreateJobApplicationDto { CoverLetter = "Applying" });
        applyResponse.EnsureSuccessStatusCode();
        var application = await applyResponse.Content.ReadFromJsonAsync<JobApplicationDto>();

        return (recruiterToken, application!);
    }

    [Fact]
    public async Task Create_ReturnsOk_WhenOwningRecruiterCreatesStage()
    {
        var (recruiterToken, application) = await SetupApplicationAsync("1");
        TestHelpers.AttachToken(_client, recruiterToken);

        var response = await _client.PostAsJsonAsync(
            $"/api/jobapplications/{application.Id}/interview-stages",
            new CreateInterviewStageDto { StageName = "Phone Screen" });

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<InterviewStageDto>();
        Assert.Equal("Phone Screen", result!.StageName);
    }

    [Fact]
    public async Task Create_ReturnsForbidden_WhenCandidateTriesToCreateStage()
    {
        var (_, application) = await SetupApplicationAsync("2");

        var candidateToken = await TestHelpers.RegisterAndLoginCandidateAsync(_client, "isintruder@test.com", "isintruder");
        TestHelpers.AttachToken(_client, candidateToken);

        var response = await _client.PostAsJsonAsync(
            $"/api/jobapplications/{application.Id}/interview-stages",
            new CreateInterviewStageDto { StageName = "Should Not Work" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetByJobApplication_ReturnsOk_WithCreatedStages()
    {
        var (recruiterToken, application) = await SetupApplicationAsync("3");
        TestHelpers.AttachToken(_client, recruiterToken);

        await _client.PostAsJsonAsync(
            $"/api/jobapplications/{application.Id}/interview-stages",
            new CreateInterviewStageDto { StageName = "Technical" });

        var response = await _client.GetAsync($"/api/jobapplications/{application.Id}/interview-stages");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<InterviewStageDto>>();
        Assert.Single(result!);
    }
}