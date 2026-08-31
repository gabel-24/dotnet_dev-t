using JobApplicationApi.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using JobApplicationApi.Dtos;


[Trait("Category", "Unit")]
public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock = new();
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task RegisterCandidate_ReturnsOk_WhenSuccessful()
    {
        var request = new RegisterCandidateDto();
        var response = new AuthDto { Token = "token123" };
        _authServiceMock.Setup(s => s.RegisterCandidateAsync(request)).ReturnsAsync(response);

        var result = await _controller.RegisterCandidate(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task RegisterCandidate_ReturnsBadRequest_WhenServiceReturnsNull()
    {
        var request = new RegisterCandidateDto();
        _authServiceMock.Setup(s => s.RegisterCandidateAsync(request)).ReturnsAsync((AuthDto?)null);

        var result = await _controller.RegisterCandidate(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RegisterRecruiter_ReturnsOk_WhenSuccessful()
    {
        var request = new RegisterRecruiterDto();
        var response = new AuthDto { Token = "token123" };
        _authServiceMock.Setup(s => s.RegisterRecruiterAsync(request)).ReturnsAsync(response);

        var result = await _controller.RegisterRecruiter(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task RegisterRecruiter_ReturnsBadRequest_WhenServiceReturnsNull()
    {
        var request = new RegisterRecruiterDto();
        _authServiceMock.Setup(s => s.RegisterRecruiterAsync(request)).ReturnsAsync((AuthDto?)null);

        var result = await _controller.RegisterRecruiter(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenCredentialsAreValid()
    {
        var request = new LoginDto();
        var response = new AuthDto { Token = "token123" };
        _authServiceMock.Setup(s => s.LoginAsync(request)).ReturnsAsync(response);

        var result = await _controller.Login(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        var request = new LoginDto();
        _authServiceMock.Setup(s => s.LoginAsync(request)).ReturnsAsync((AuthDto?)null);

        var result = await _controller.Login(request);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}