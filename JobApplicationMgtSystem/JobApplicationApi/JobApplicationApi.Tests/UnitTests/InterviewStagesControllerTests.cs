using JobApplicationApi.Controllers;
using JobApplicationApi.Dtos;
using JobApplicationApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

[Trait("Category", "Unit")]
public class InterviewStagesControllerTests
{
    private readonly Mock<IInterviewStageService> _serviceMock = new();
    private readonly Mock<IRecruiterService> _rServiceMock = new();
    private readonly InterviewStageController _controller;

    public InterviewStagesControllerTests()
    {
        _controller = new InterviewStageController(_serviceMock.Object, _rServiceMock.Object);
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
    public async Task GetByJobApplication_ReturnsOk_WithList()
    {
        var stages = new List<InterviewStageDto>();
        _serviceMock.Setup(s => s.GetByJobApplicationIdAsync(1)).ReturnsAsync(stages);

        var result = await _controller.GetByJobApplication(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(stages, okResult.Value);
    }

    [Fact]
    public async Task Create_ReturnsNotFound_WhenRecruiterProfileMissing()
    {
        SetUser("user-1");
        _rServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync((RecruiterDto?)null);

        var result = await _controller.Create(1, new CreateInterviewStageDto());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsOk_WhenSuccessful()
    {
        SetUser("user-1");
        var recruiter = new RecruiterDto { Id = 5 };
        var request = new CreateInterviewStageDto();
        var created = new InterviewStageDto { Id = 9 };

        _rServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync(recruiter);
        _serviceMock.Setup(s => s.CreateAsync(5, 1, request)).ReturnsAsync(created);

        var result = await _controller.Create(1, request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(created, okResult.Value);
    }

    [Fact]
    public async Task Create_ReturnsNotFound_WhenServiceReturnsNull()
    {
        SetUser("user-1");
        var recruiter = new RecruiterDto { Id = 5 };
        var request = new CreateInterviewStageDto();

        _rServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync(recruiter);
        _serviceMock.Setup(s => s.CreateAsync(5, 1, request)).ReturnsAsync((InterviewStageDto?)null);

        var result = await _controller.Create(1, request);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenSuccessful()
    {
        SetUser("user-1");
        var recruiter = new RecruiterDto { Id = 5 };
        var request = new UpdateInterviewStageDto();

        _rServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync(recruiter);
        _serviceMock.Setup(s => s.UpdateAsync(5, 1, request)).ReturnsAsync(true);

        var result = await _controller.Update(1, request);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenServiceFails()
    {
        SetUser("user-1");
        var recruiter = new RecruiterDto { Id = 5 };
        var request = new UpdateInterviewStageDto();

        _rServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync(recruiter);
        _serviceMock.Setup(s => s.UpdateAsync(5, 1, request)).ReturnsAsync(false);

        var result = await _controller.Update(1, request);

        Assert.IsType<NotFoundResult>(result);
    }
}