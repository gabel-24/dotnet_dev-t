using AutoMapper;
using JobApplicationApi.Dtos;
using JobApplicationApi.Models;
using JobApplicationApi.Repositories;

namespace JobApplicationApi.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _repository;
        private readonly IJobPostingRepository _jprepository;
        private readonly ICandidateRepository _crepository;
        private readonly IMapper _mapper;

        public JobApplicationService(IJobApplicationRepository repository, IJobPostingRepository jprepository, ICandidateRepository crepository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _jprepository = jprepository;
            _crepository = crepository;

        }

        public async Task<JobApplicationDto?> GetByIdAsync(int id, string userId, bool isRecruiter)
        {
            var jobapplication = await _repository.GetByIdAsync(id);
            if (jobapplication == null || (isRecruiter
                ? jobapplication.JobPosting.Recruiter.UserId != userId
                : jobapplication.Candidate.UserId != userId))
                return null;

            var dto = _mapper.Map<JobApplicationDto>(jobapplication);
            if (!isRecruiter)
                foreach (var stage in dto.InterviewStages)
                    stage.Notes = null;
            return dto;
        }
        public async Task<PagedResponse<JobApplicationSummaryDto>> GetByCandidateAsync(int candidateId, int pageNumber, int pageSize)
        {
            Pagination.Validate(pageNumber, pageSize);
            var (items, totalCount) = await _repository.GetByCandidateIdAsync(candidateId, pageNumber, pageSize);

            return new PagedResponse<JobApplicationSummaryDto>
            {
                Items = _mapper.Map<List<JobApplicationSummaryDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<PagedResponse<JobApplicationSummaryDto>> GetByJobPostingAsync(int recruiterId, int jobPostingId, int pageNumber, int pageSize)
        {
            Pagination.Validate(pageNumber, pageSize);
            var jobPosting = await _jprepository.GetByIdAsync(jobPostingId);

            if (jobPosting == null || jobPosting.RecruiterId != recruiterId)
            {
                return new PagedResponse<JobApplicationSummaryDto>
                {
                    Items = new List<JobApplicationSummaryDto>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = 0
                };
            }

            var (items, totalCount) = await _repository.GetByJobPostingIdAsync(jobPostingId, pageNumber, pageSize);

            return new PagedResponse<JobApplicationSummaryDto>
            {
                Items = _mapper.Map<List<JobApplicationSummaryDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
        public async Task<JobApplicationDto> CreateAsync(int candidateId, CreateJobApplicationDto request)
        {
            var candidate = await _crepository.GetByIdAsync(candidateId);

            if(candidate == null)
            {
                throw new BadHttpRequestException("Candidate not found.", StatusCodes.Status404NotFound);
            }
            
            var posting = await _jprepository.GetByIdAsync(request.JobPostingId);
            if (posting == null)
                throw new BadHttpRequestException("Job posting not found.", StatusCodes.Status404NotFound);
            if (!posting.IsActive || posting.ClosingDate < DateOnly.FromDateTime(DateTime.UtcNow))
                throw new BadHttpRequestException("This job posting is closed.");
            if (await _repository.ExistsAsync(candidateId, request.JobPostingId))
                throw new BadHttpRequestException("You have already applied to this job.", StatusCodes.Status409Conflict);

            var jobApplication = _mapper.Map<JobApplication>(request);

            jobApplication.CandidateProfileId = candidateId;
            jobApplication.ResumeSnapshotUrl = candidate.ResumeUrl ?? string.Empty;

            await _repository.AddAsync(jobApplication);

            return _mapper.Map<JobApplicationDto>(jobApplication);
        }
        public async Task<bool> UpdateStatusAsync(int recruiterId, int jobApplicationId, UpdateJobApplicationDto request)
        {
            if (!Enum.IsDefined(request.Status))
                throw new BadHttpRequestException("Invalid application status.");
            var jobApplication = await _repository.GetByIdAsync(jobApplicationId);

            if(jobApplication == null)
            {
                return false;
            }

            if(jobApplication.JobPosting.RecruiterId != recruiterId)
            {
                return false;
            }

            _mapper.Map(request, jobApplication);
            await _repository.UpdateAsync(jobApplication);

            return true;
        }
    }
}
