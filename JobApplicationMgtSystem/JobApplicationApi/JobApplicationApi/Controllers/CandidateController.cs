using JobApplicationApi.Dtos;
using JobApplicationApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


[ApiController]
[Route("api/candidates")]
public class CandidatesController : ControllerBase
{
    private readonly ICandidateService _service;

    public CandidatesController(ICandidateService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) 
    {
        var candidate = await _service.GetByIdAsync(id);

        if(candidate == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(candidate);
        }
    }

    [Authorize(Roles = "Candidate")]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe() 
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var candidate = await _service.GetByUserIdAsync(userId!);

        if(candidate == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(candidate);
        }
    }

    [Authorize(Roles = "Candidate")]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateCandidateDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var candidate = await _service.GetByUserIdAsync(userId);

        var success = await _service.UpdateAsync(userId, request);

        if (!success)
        {
            return NotFound();
        }
        else
        {
            return NoContent();
        }
    }
}
