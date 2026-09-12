using JobApplicationApi.Services;
using Microsoft.AspNetCore.Mvc;
using JobApplicationApi.Dtos;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register/candidate")]
    public async Task<IActionResult> RegisterCandidate(RegisterCandidateDto request)
    {
        try
        {
            var result = await _authService.RegisterCandidateAsync(request);
            if (result == null)
                return BadRequest(new { message = "Registration failed. Please check your details and try again." });

            return Ok(result);
        }
        catch (RegistrationException exception)
        {
            return StatusCode(exception.IsDuplicate ? 409 : 400, new
            {
                message = exception.Message,
                errors = exception.Errors
            });
        }
    }

    [HttpPost("register/recruiter")]
    public async Task<IActionResult> RegisterRecruiter(RegisterRecruiterDto request)
    {
        try
        {
            var result = await _authService.RegisterRecruiterAsync(request);
            if (result == null)
                return BadRequest(new { message = "Registration failed. Please check your details and try again." });

            return Ok(result);
        }
        catch (RegistrationException exception)
        {
            return StatusCode(exception.IsDuplicate ? 409 : 400, new
            {
                message = exception.Message,
                errors = exception.Errors
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (result == null)
            return Unauthorized(new { message = "Invalid email or password." });

        return Ok(result);
    }
}