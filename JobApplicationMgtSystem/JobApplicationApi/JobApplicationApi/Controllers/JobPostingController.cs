using JobApplicationApi.Dtos;
using JobApplicationApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobApplicationApi.Controllers
{
    [ApiController]
    [Route("api/jobpostings")]
    public class JobPostingController : Controller
    {
        private readonly IJobPostingService _service;
        private readonly IRecruiterService _rService;


        public JobPostingController(IJobPostingService service, IRecruiterService rservice)
        {
            _service = service;
            _rService = rservice;

        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(int pageNumber = 1, int pageSize = 10, string? location = null, string? employmentType = null, string? keyword = null)
        {
            var result = await _service.GetPagedAsync(pageNumber, pageSize, location, employmentType, keyword);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var jobPosting = await _service.GetByIdAsync(id);
            return jobPosting == null ? NotFound() : Ok(jobPosting);
        }

        [Authorize(Roles = "Recruiter")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateJobPostingDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var recruiter = await _rService.GetByUserIdAsync(userId!);

            if(recruiter == null)
            {
                return NotFound("Recruiter profile not found.");
            }

            var jobPosting = await _service.CreateAsync(recruiter.Id, request);

            return CreatedAtAction(nameof(GetById), new { id = jobPosting.Id }, jobPosting);
        }

        [Authorize(Roles = "Recruiter")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateJobPostingDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var recruiter = await _rService.GetByUserIdAsync(userId!);

            if (recruiter == null)
                return NotFound("Recruiter profile not found.");

            var success = await _service.UpdateAsync(recruiter.Id, id, request);

            return success == false ? NotFound() : NoContent();
        }
    }
}
