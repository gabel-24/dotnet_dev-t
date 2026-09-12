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
    [Theory]
    [InlineData("DuplicateEmail", "Email", 409)]
    [InlineData("DuplicateUserName", "Username", 409)]
    [InlineData("PasswordRequiresDigit", "Password", 400)]
    public async Task Registration_ReturnsSpecificErrors_ForBothRoles(string code, string field, int status)
    {
        var failure = new RegistrationException(new[] {
            new Microsoft.AspNetCore.Identity.IdentityError { Code = code, Description = "Password must contain a digit." }
        });
        _authServiceMock.Setup(s => s.RegisterCandidateAsync(It.IsAny<RegisterCandidateDto>())).ThrowsAsync(failure);
        _authServiceMock.Setup(s => s.RegisterRecruiterAsync(It.IsAny<RegisterRecruiterDto>())).ThrowsAsync(failure);

        var results = new[] {
            await _controller.RegisterCandidate(new RegisterCandidateDto()),
            await _controller.RegisterRecruiter(new RegisterRecruiterDto())
        };
        foreach (var result in results)
        {
            var response = Assert.IsType<ObjectResult>(result);
            Assert.Equal(status, response.StatusCode);
            var json = System.Text.Json.JsonSerializer.SerializeToElement(response.Value);
            Assert.Equal(failure.Errors[field][0], json.GetProperty("errors").GetProperty(field)[0].GetString());
        }
    }
}