using JobApplicationApi.Controllers;
using JobApplicationApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;
using JobApplicationApi.Dtos;

[Trait("Category", "Unit")]
public class RecruitersControllerTests
{
    private readonly Mock<IRecruiterService> _recruiterServiceMock = new();
    private readonly RecruiterController _controller;

    public RecruitersControllerTests()
    {
        _controller = new RecruiterController(_recruiterServiceMock.Object);
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
    public async Task GetById_ReturnsOk_WhenRecruiterExists()
    {
        var recruiter = new RecruiterDto { Id = 1 };
        _recruiterServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(recruiter);

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(recruiter, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenRecruiterMissing()
    {
        _recruiterServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((RecruiterDto?)null);

        var result = await _controller.GetById(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetMe_ReturnsOk_WhenRecruiterExists()
    {
        SetUser("user-1");
        var recruiter = new RecruiterDto { Id = 1 };
        _recruiterServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync(recruiter);

        var result = await _controller.GetMe();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(recruiter, okResult.Value);
    }

    [Fact]
    public async Task UpdateMe_ReturnsNoContent_WhenUpdateSucceeds()
    {
        SetUser("user-1");
        var request = new UpdateRecruiterDto();
        _recruiterServiceMock.Setup(s => s.UpdateAsync("user-1", request)).ReturnsAsync(true);

        var result = await _controller.UpdateMe(request);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateMe_ReturnsNotFound_WhenUpdateFails()
    {
        SetUser("user-1");
        var request = new UpdateRecruiterDto();
        _recruiterServiceMock.Setup(s => s.UpdateAsync("user-1", request)).ReturnsAsync(false);

        var result = await _controller.UpdateMe(request);

        Assert.IsType<NotFoundResult>(result);
    }
}