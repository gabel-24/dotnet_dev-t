using JobApplicationApi.Dtos;
using JobApplicationApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobApplicationApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class JobApplicationController : Controller
    {
        private readonly IJobApplicationService _service;
        private readonly IRecruiterService _rservice;
        private readonly ICandidateService _cservice;

        public JobApplicationController(IJobApplicationService service, IRecruiterService rservice, ICandidateService cservice)
        {
            _service = service;
            _rservice = rservice;
            _cservice = cservice;
        }


        [Authorize(Roles = "Candidate")]
        [HttpPost("jobpostings/{jobPostingId}/applications")]
        public async Task<IActionResult> Create(int jobPostingId, CreateJobApplicationDto request)
        {
            var candidateId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var candidate = await _cservice.GetByUserIdAsync(candidateId!);

            if(candidate == null)
            {
                return NotFound("Candidate doesnt exist");
            }

            var jobApplication = await _service.CreateAsync(candidate!.Id, request);
            return Ok(jobApplication);
        }

        [Authorize]
        [HttpGet("jobapplications/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var jobApplication = await _service.GetByIdAsync(id);

            return jobApplication == null ? NotFound() : Ok(jobApplication);
        }

        [Authorize(Roles = "Recruiter")]
        [HttpPut("jobapplications/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateJobApplicationDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var recruiter = await _rservice.GetByUserIdAsync(userId!);

            if(recruiter == null)
            {
                return NotFound("Recruiter profile not found");
            }

            var success = await _service.UpdateStatusAsync(recruiter.Id, id, request);

            return success == false ? NotFound() : NoContent();
        }

        [Authorize(Roles = "Candidate")]
        [HttpGet("candidates/me/applications")]
        public async Task<IActionResult> GetMyApplications(int pageNumber = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var candidate = await _cservice.GetByUserIdAsync(userId!);

            if (candidate == null)
                return NotFound("Candidate profile not found.");

            var jobApplications = await _service.GetByCandidateAsync(candidate.Id, pageNumber, pageSize);

            return Ok(jobApplications);
        }

        [Authorize(Roles = "Recruiter")]
        [HttpGet("jobpostings/{jobPostingId}/applications")]
        public async Task<IActionResult> GetApplicationsForPosting(int jobPostingId, int pageNumber = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var recruiter = await _rservice.GetByUserIdAsync(userId!);

            if (recruiter == null)
                return NotFound("Recruiter profile not found.");

            var result = await _service.GetByJobPostingAsync(recruiter.Id, jobPostingId, pageNumber, pageSize);

            return Ok(result);
        }
    }
}   