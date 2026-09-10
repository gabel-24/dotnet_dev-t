using AutoMapper;
using JobApplicationApi.Dtos;
using JobApplicationApi.Models;
using JobApplicationApi.Repositories;
using JobApplicationApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

[Trait("Category", "Unit")]
public class JobApplicationServiceTests
{
    private readonly Mock<IJobApplicationRepository> applications = new();
    private readonly Mock<IJobPostingRepository> postings = new();
    private readonly Mock<ICandidateRepository> candidates = new();
    private readonly IMapper mapper = new MapperConfiguration(c => c.AddProfile<MappingProfile>(), NullLoggerFactory.Instance).CreateMapper();
    private JobApplicationService Service => new(applications.Object, postings.Object, candidates.Object, mapper);

    [Theory]
    [InlineData("outsider", false, false)]
    [InlineData("outsider", true, false)]
    [InlineData("candidate", false, true)]
    [InlineData("recruiter", true, true)]
    public async Task Details_CheckOwnershipAndHideCandidateNotes(string userId, bool recruiter, bool allowed)
    {
        applications.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new JobApplication
        {
            Candidate = new Candidate { UserId = "candidate", User = new ApplicationUser { UserName = "Alice" } },
            JobPosting = new JobPosting { Recruiter = new Recruiter { UserId = "recruiter", User = new ApplicationUser() } },
            InterviewStages = new List<InterviewStage> { new() { Notes = "Private assessment" } }
        });
        var result = await Service.GetByIdAsync(1, userId, recruiter);
        if (!allowed) { Assert.Null(result); return; }
        Assert.NotNull(result);
        Assert.Equal(recruiter ? "Private assessment" : null, result.InterviewStages.Single().Notes);
    }

    [Theory]
    [InlineData("missing", 404)]
    [InlineData("inactive", 400)]
    [InlineData("expired", 400)]
    [InlineData("duplicate", 409)]
    public async Task Create_RejectsInvalidApplicationsBeforeSaving(string scenario, int status)
    {
        candidates.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Candidate());
        postings.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(scenario == "missing" ? null : new JobPosting
        {
            IsActive = scenario != "inactive",
            ClosingDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(scenario == "expired" ? -1 : 1)
        });
        applications.Setup(r => r.ExistsAsync(1, 2)).ReturnsAsync(scenario == "duplicate");
        var error = await Assert.ThrowsAsync<BadHttpRequestException>(() => Service.CreateAsync(1, new CreateJobApplicationDto { JobPostingId = 2 }));
        Assert.Equal(status, error.StatusCode);
        applications.Verify(r => r.AddAsync(It.IsAny<JobApplication>()), Times.Never);
    }

    [Fact]
    public void ProfileMapping_ReturnsIdentityAndHeadline()
    {
        var user = new ApplicationUser { UserName = "Alice", Email = "alice@example.com" };
        var candidate = mapper.Map<CandidateDto>(new Candidate { User = user, Headline = "Developer" });
        Assert.Equal("Alice", candidate.Username);
        Assert.Equal(user.Email, candidate.Email);
        Assert.Equal("Developer", candidate.Headline);
        var recruiter = mapper.Map<RecruiterDto>(new Recruiter { User = user });
        Assert.Equal("Alice", recruiter.Username);
        Assert.Equal(user.Email, recruiter.Email);
    }
    [Fact]
    public async Task PostingApplicants_ReturnSubmittedCvOnlyToOwningRecruiter()
    {
        postings.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new JobPosting { RecruiterId = 5 });
        applications.Setup(r => r.GetByJobPostingIdAsync(2, 1, 10)).ReturnsAsync((new List<JobApplication>
        {
            new() {
                ResumeSnapshotUrl = "https://example.com/submitted-cv.pdf",
                Candidate = new Candidate { ResumeUrl = "https://example.com/new-cv.pdf", User = new ApplicationUser() },
                JobPosting = new JobPosting()
            }
        }, 1));

        var result = await Service.GetByJobPostingAsync(5, 2, 1, 10);
        Assert.Equal("https://example.com/submitted-cv.pdf", result.Items.Single().ResumeSnapshotUrl);
        var denied = await Service.GetByJobPostingAsync(6, 2, 1, 10);
        Assert.Empty(denied.Items);
    }
}
