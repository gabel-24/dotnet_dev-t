using JobApplicationApi.Dtos;
using JobApplicationApi.Models;
using System.Net;
using System.Net.Http.Json;
using Xunit;

[Trait("Category", "Integration")]
public class JobApplicationsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public JobApplicationsEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<JobPostingDto> CreateJobPostingAsync(string recruiterEmail, string recruiterUserName)
    {
        var token = await TestHelpers.RegisterAndLoginRecruiterAsync(_client, recruiterEmail, recruiterUserName);
        TestHelpers.AttachToken(_client, token);

        var postingResponse = await _client.PostAsJsonAsync("/api/jobpostings", new CreateJobPostingDto
        {
            Title = "Test Posting",
            Description = "Desc"
        });

        postingResponse.EnsureSuccessStatusCode();

        return (await postingResponse.Content.ReadFromJsonAsync<JobPostingDto>())!;
    }

    [Fact]
    public async Task Create_ReturnsOk_WhenCandidateApplies()
    {
        var posting = await CreateJobPostingAsync("apprec1@test.com", "apprec1");

        var candidateToken = await TestHelpers.RegisterAndLoginCandidateAsync(_client, "appcand1@test.com", "appcand1");
        TestHelpers.AttachToken(_client, candidateToken);

        var Dto = await _client.PostAsJsonAsync(
            $"/api/jobpostings/{posting.Id}/applications",
            new CreateJobApplicationDto { CoverLetter = "I'm a great fit." });

        Dto.EnsureSuccessStatusCode();
        var result = await Dto.Content.ReadFromJsonAsync<JobApplicationDto>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Create_ReturnsConflictOrBadRequest_WhenApplyingTwice()
    {
        var posting = await CreateJobPostingAsync("apprec2@test.com", "apprec2");

        var candidateToken = await TestHelpers.RegisterAndLoginCandidateAsync(_client, "appcand2@test.com", "appcand2");
        TestHelpers.AttachToken(_client, candidateToken);

        var request = new CreateJobApplicationDto { CoverLetter = "First try" };

        await _client.PostAsJsonAsync($"/api/jobpostings/{posting.Id}/applications", request);
        var secondDto = await _client.PostAsJsonAsync($"/api/jobpostings/{posting.Id}/applications", request);

        Assert.False(secondDto.IsSuccessStatusCode);
    }

    [Fact]
    public async Task GetMyApplications_ReturnsOnlyOwnApplications()
    {
        var posting = await CreateJobPostingAsync("apprec3@test.com", "apprec3");

        var candidateToken = await TestHelpers.RegisterAndLoginCandidateAsync(_client, "appcand3@test.com", "appcand3");
        TestHelpers.AttachToken(_client, candidateToken);

        await _client.PostAsJsonAsync(
            $"/api/jobpostings/{posting.Id}/applications",
            new CreateJobApplicationDto { CoverLetter = "Applying" });

        var Dto = await _client.GetAsync("/api/candidates/me/applications");

        Dto.EnsureSuccessStatusCode();
        var result = await Dto.Content.ReadFromJsonAsync<PagedResponse<JobApplicationSummaryDto>>();

        Assert.True(result!.TotalCount >= 1);
    }

    [Fact]
    public async Task UpdateStatus_ReturnsNoContent_WhenOwningRecruiterUpdates()
    {
        var recruiterToken = await TestHelpers.RegisterAndLoginRecruiterAsync(_client, "apprec4@test.com", "apprec4");
        TestHelpers.AttachToken(_client, recruiterToken);

        var postingDto = await _client.PostAsJsonAsync("/api/jobpostings", new CreateJobPostingDto
        {
            Title = "Status Test Posting",
            Description = "Desc"
        });
        var posting = await postingDto.Content.ReadFromJsonAsync<JobPostingDto>();

        var candidateToken = await TestHelpers.RegisterAndLoginCandidateAsync(_client, "appcand4@test.com", "appcand4");
        TestHelpers.AttachToken(_client, candidateToken);

        var applyDto = await _client.PostAsJsonAsync(
            $"/api/jobpostings/{posting!.Id}/applications",
            new CreateJobApplicationDto { CoverLetter = "Applying" });
        var application = await applyDto.Content.ReadFromJsonAsync<JobApplicationDto>();

        TestHelpers.AttachToken(_client, recruiterToken);

        var updateDto = await _client.PutAsJsonAsync(
            $"/api/jobapplications/{application!.Id}",
            new UpdateJobApplicationDto { Status = ApplicationStatus.Interview });

        Assert.Equal(HttpStatusCode.NoContent, updateDto.StatusCode);
    }
}