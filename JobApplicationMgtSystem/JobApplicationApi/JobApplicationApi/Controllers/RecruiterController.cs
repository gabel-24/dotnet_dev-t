using JobApplicationApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JobApplicationApi.Dtos;

namespace JobApplicationApi.Controllers
{
    [ApiController]
    [Route("api/recruiters")]
    public class RecruiterController : ControllerBase
    {
        private readonly IRecruiterService _service;

        public RecruiterController(IRecruiterService service)
        {
            _service = service;
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(RecruiterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Recruiter ID must be greater than zero.");
            }

            var recruiter = await _service.GetByIdAsync(id);
            return recruiter == null ? NotFound() : Ok(recruiter);
        }

        [Authorize(Roles = "Recruiter")]
        [HttpGet("me")]
        [ProducesResponseType(typeof(RecruiterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var recruiter = await _service.GetByUserIdAsync(userId);

            return recruiter == null ? NotFound() : Ok(recruiter);
        }

        [Authorize(Roles = "Recruiter")]
        [HttpPut("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMe(UpdateRecruiterDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var success = await _service.UpdateAsync(userId, request);

            return success ? NoContent() : NotFound();
        }
    }
}
