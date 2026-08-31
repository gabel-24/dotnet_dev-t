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
        var result = await _authService.RegisterCandidateAsync(request);

        if (result == null)
            return BadRequest("Registration failed. Email may already be in use or password does not meet requirements.");

        return Ok(result);
    }

    [HttpPost("register/recruiter")]
    public async Task<IActionResult> RegisterRecruiter(RegisterRecruiterDto request)
    {
        var result = await _authService.RegisterRecruiterAsync(request);

        if (result == null)
            return BadRequest("Registration failed. Email may already be in use or password does not meet requirements.");

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (result == null)
            return Unauthorized("Invalid email or password.");

        return Ok(result);
    }
}