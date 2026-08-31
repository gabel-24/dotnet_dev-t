using JobApplicationApi.Dtos;
using JobApplicationApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobApplicationApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class InterviewStageController : Controller
    {
        private readonly IInterviewStageService _service;
        private readonly IRecruiterService _rservice;

        public InterviewStageController(IInterviewStageService service, IRecruiterService rservice)
        {
            _service = service;
            _rservice = rservice;
        }


        [Authorize]
        [HttpGet("jobapplications/{jobApplicationId}/interview-stages")]
        public async Task<IActionResult> GetByJobApplication(int jobApplicationId)
        {
            var interviewStages = await _service.GetByJobApplicationIdAsync(jobApplicationId);

            return interviewStages == null ? NotFound() : Ok(interviewStages);
        }

        [Authorize(Roles = "Recruiter")]
        [HttpPost("jobapplications/{jobApplicationId}/interview-stages")]
        public async Task<IActionResult> Create(int jobApplicationId, CreateInterviewStageDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var recruiter = await _rservice.GetByUserIdAsync(userId!);

            if(recruiter == null)
            {
                return NotFound("No recruiter profile found");
            }

            var interviewStage = await _service.CreateAsync(recruiter.Id, jobApplicationId, request);

            return interviewStage == null ? NotFound() : Ok(interviewStage);
        }

        [Authorize(Roles = "Recruiter")]
        [HttpPut("interview-stages/{id}")]
        public async Task<IActionResult> Update(int id, UpdateInterviewStageDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var recruiter = await _rservice.GetByUserIdAsync(userId!);

            if (recruiter == null)
            {
                return NotFound("No recruiter profile found");
            }

            var success = await _service.UpdateAsync(recruiter.Id, id, request);

            return success == false ? NotFound() : NoContent();
        }

        [Authorize(Roles = "Recruiter")]
        [HttpDelete("interview-stages/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var recruiter = await _rservice.GetByUserIdAsync(userId!);

            if (recruiter == null)
            {
                return NotFound("No recruiter profile found");
            }

            var success = await _service.DeleteAsync(recruiter.Id, id);

            return success == false ? NotFound() : NoContent();
        }
    }
}
