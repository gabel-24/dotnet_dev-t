using JobApplicationApi.Controllers;
using JobApplicationApi.Dtos;
using JobApplicationApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

[Trait("Category", "Unit")]
public class JobApplicationControllerTests
{
    private readonly Mock<IJobApplicationService> _serviceMock = new();
    private readonly Mock<IRecruiterService> _rServiceMock = new();
    private readonly Mock<ICandidateService> _cServiceMock = new();
    private readonly JobApplicationController _controller;

    public JobApplicationControllerTests()
    {
        _controller = new JobApplicationController(_serviceMock.Object, _rServiceMock.Object, _cServiceMock.Object);
    }

    private void SetUser(string userId)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    [Fact]
    public async Task Create_ReturnsNotFound_WhenCandidateProfileMissing()
    {
        SetUser("user-1");
        _cServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync((CandidateDto?)null);

        var result = await _controller.Create(1, new CreateJobApplicationDto());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsOk_WhenSuccessful()
    {
        SetUser("user-1");
        var candidate = new CandidateDto { Id = 3 };
        var request = new CreateJobApplicationDto();
        var created = new JobApplicationDto { Id = 7 };

        _cServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync(candidate);
        _serviceMock.Setup(s => s.CreateAsync(3, request)).ReturnsAsync(created);

        var result = await _controller.Create(1, request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(created, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((JobApplicationDto?)null);

        var result = await _controller.GetById(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateStatus_ReturnsNotFound_WhenRecruiterProfileMissing()
    {
        SetUser("user-1");
        _rServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync((RecruiterDto?)null);

        var result = await _controller.UpdateStatus(1, new UpdateJobApplicationDto());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateStatus_ReturnsNoContent_WhenSuccessful()
    {
        SetUser("user-1");
        var recruiter = new RecruiterDto { Id = 5 };
        var request = new UpdateJobApplicationDto();

        _rServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync(recruiter);
        _serviceMock.Setup(s => s.UpdateStatusAsync(5, 1, request)).ReturnsAsync(true);

        var result = await _controller.UpdateStatus(1, request);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetMyApplications_ReturnsNotFound_WhenCandidateProfileMissing()
    {
        SetUser("user-1");
        _cServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync((CandidateDto?)null);

        var result = await _controller.GetMyApplications();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetMyApplications_ReturnsOk_WhenSuccessful()
    {
        SetUser("user-1");
        var candidate = new CandidateDto { Id = 3 };
        var paged = new PagedResponse<JobApplicationSummaryDto> { Items = new() };

        _cServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync(candidate);
        _serviceMock.Setup(s => s.GetByCandidateAsync(3, 1, 10)).ReturnsAsync(paged);

        var result = await _controller.GetMyApplications();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(paged, okResult.Value);
    }

    [Fact]
    public async Task GetApplicationsForPosting_ReturnsNotFound_WhenRecruiterProfileMissing()
    {
        SetUser("user-1");
        _rServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync((RecruiterDto?)null);

        var result = await _controller.GetApplicationsForPosting(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetApplicationsForPosting_ReturnsOk_WhenSuccessful()
    {
        SetUser("user-1");
        var recruiter = new RecruiterDto { Id = 5 };
        var paged = new PagedResponse<JobApplicationSummaryDto> { Items = new() };

        _rServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync(recruiter);
        _serviceMock.Setup(s => s.GetByJobPostingAsync(5, 1, 1, 10)).ReturnsAsync(paged);

        var result = await _controller.GetApplicationsForPosting(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(paged, okResult.Value);
    }
}