using JobApplicationApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JobApplicationApi.Dtos;

namespace JobApplicationApi.Controllers
{
    public class RecruiterController : Controller
    {
        private readonly IRecruiterService _service;

        public RecruiterController(IRecruiterService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var recruiter = await _service.GetByIdAsync(id);

            if(recruiter == null)
            {
                return NotFound();
            }

            else
            {
                return Ok(recruiter);
            }
        }

        [Authorize(Roles = "Recruiter")]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var recruiter = await _service.GetByUserIdAsync(userId!);

            return recruiter == null ? NotFound() : Ok(recruiter);
        }

        [Authorize(Roles = "Recruiter")]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe(UpdateRecruiterDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var success = await _service.UpdateAsync(userId!, request);

            return success == false ? NotFound() : NoContent();

        }
    }
}
