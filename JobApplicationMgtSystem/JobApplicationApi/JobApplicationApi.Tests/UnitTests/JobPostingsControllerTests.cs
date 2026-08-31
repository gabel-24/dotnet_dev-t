using JobApplicationApi.Controllers;
using JobApplicationApi.Dtos;
using JobApplicationApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

[Trait("Category", "Unit")]
public class JobPostingsControllerTests
{
    private readonly Mock<IJobPostingService> _jobPostingServiceMock = new();
    private readonly Mock<IRecruiterService> _recruiterServiceMock = new();
    private readonly JobPostingController _controller;

    public JobPostingsControllerTests()
    {
        _controller = new JobPostingController(_jobPostingServiceMock.Object, _recruiterServiceMock.Object);
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
    public async Task GetPaged_ReturnsOk_WithPagedResult()
    {
        var paged = new PagedResponse<JobPostingSummaryDto> { Items = new() };
        _jobPostingServiceMock
            .Setup(s => s.GetPagedAsync(1, 10, null, null, null))
            .ReturnsAsync(paged);

        var result = await _controller.GetPaged();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(paged, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        _jobPostingServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((JobPostingDto?)null);

        var result = await _controller.GetById(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsNotFound_WhenRecruiterProfileMissing()
    {
        SetUser("user-1");
        _recruiterServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync((RecruiterDto?)null);

        var result = await _controller.Create(new CreateJobPostingDto());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenSuccessful()
    {
        SetUser("user-1");
        var recruiter = new RecruiterDto { Id = 5 };
        var request = new CreateJobPostingDto();
        var created = new JobPostingDto { Id = 10 };

        _recruiterServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync(recruiter);
        _jobPostingServiceMock.Setup(s => s.CreateAsync(5, request)).ReturnsAsync(created);

        var result = await _controller.Create(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(created, createdResult.Value);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenSuccessful()
    {
        SetUser("user-1");
        var recruiter = new RecruiterDto { Id = 5 };
        var request = new UpdateJobPostingDto();

        _recruiterServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync(recruiter);
        _jobPostingServiceMock.Setup(s => s.UpdateAsync(5, 10, request)).ReturnsAsync(true);

        var result = await _controller.Update(10, request);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenUpdateFails()
    {
        SetUser("user-1");
        var recruiter = new RecruiterDto { Id = 5 };
        var request = new UpdateJobPostingDto();

        _recruiterServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync(recruiter);
        _jobPostingServiceMock.Setup(s => s.UpdateAsync(5, 10, request)).ReturnsAsync(false);

        var result = await _controller.Update(10, request);

        Assert.IsType<NotFoundResult>(result);
    }
}