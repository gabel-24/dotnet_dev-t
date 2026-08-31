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
        private readonly IMapper _mapper;

        public JobApplicationService(IJobApplicationRepository repository, IJobPostingRepository jprepository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _jprepository = jprepository;
        }

        public async Task<JobApplicationDto?> GetByIdAsync(int id)
        {
            var jobapplication = await _repository.GetByIdAsync(id);
            return jobapplication == null ? null : _mapper.Map<JobApplicationDto>(jobapplication);
        }
        public async Task<PagedResponse<JobApplicationSummaryDto>> GetByCandidateAsync(int candidateId, int pageNumber, int pageSize)
        {
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
            var jobApplication = _mapper.Map<JobApplication>(request);
            jobApplication.CandidateProfileId = candidateId;

            await _repository.AddAsync(jobApplication);

            return _mapper.Map<JobApplicationDto>(jobApplication);
        }
        public async Task<bool> UpdateStatusAsync(int recruiterId, int jobApplicationId, UpdateJobApplicationDto request)
        {
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
