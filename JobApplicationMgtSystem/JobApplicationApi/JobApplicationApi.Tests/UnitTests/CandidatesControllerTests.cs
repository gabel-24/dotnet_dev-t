using JobApplicationApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;
using JobApplicationApi.Dtos;


[Trait("Category", "Unit")]
public class CandidatesControllerTests
{
    private readonly Mock<ICandidateService> _candidateServiceMock = new();
    private readonly CandidatesController _controller;

    public CandidatesControllerTests()
    {
        _controller = new CandidatesController(_candidateServiceMock.Object);
    }

    private void SetUser(string userId)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenCandidateExists()
    {
        var candidate = new CandidateDto { Id = 1 };
        _candidateServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(candidate);

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(candidate, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenCandidateDoesNotExist()
    {
        _candidateServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((CandidateDto?)null);

        var result = await _controller.GetById(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetMe_ReturnsOk_WhenCandidateExists()
    {
        SetUser("user-1");
        var candidate = new CandidateDto { Id = 1 };
        _candidateServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync(candidate);

        var result = await _controller.GetMe();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(candidate, okResult.Value);
    }

    [Fact]
    public async Task GetMe_ReturnsNotFound_WhenCandidateProfileMissing()
    {
        SetUser("user-1");
        _candidateServiceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync((CandidateDto?)null);

        var result = await _controller.GetMe();

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateMe_ReturnsNoContent_WhenUpdateSucceeds()
    {
        SetUser("user-1");
        var request = new UpdateCandidateDto();
        _candidateServiceMock.Setup(s => s.UpdateAsync("user-1", request)).ReturnsAsync(true);

        var result = await _controller.UpdateMe(request);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateMe_ReturnsNotFound_WhenUpdateFails()
    {
        SetUser("user-1");
        var request = new UpdateCandidateDto();
        _candidateServiceMock.Setup(s => s.UpdateAsync("user-1", request)).ReturnsAsync(false);

        var result = await _controller.UpdateMe(request);

        Assert.IsType<NotFoundResult>(result);
    }
}