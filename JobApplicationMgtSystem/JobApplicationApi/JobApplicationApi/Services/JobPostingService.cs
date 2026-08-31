using AutoMapper;
using JobApplicationApi.Dtos;
using JobApplicationApi.Models;
using JobApplicationApi.Repositories;
using JobApplicationApi.Services;

public class JobPostingService : IJobPostingService
{
    private readonly IJobPostingRepository _repository;
    private readonly IMapper _mapper;

    public JobPostingService(IJobPostingRepository jobPostingRepository, IMapper mapper)
    {
        _repository = jobPostingRepository;
        _mapper = mapper;
    }

    public async Task<JobPostingDto?> GetByIdAsync(int id)
    {
        var jobPosting = await _repository.GetByIdAsync(id);
        return jobPosting == null ? null : _mapper.Map<JobPostingDto>(jobPosting);
    }

    public async Task<PagedResponse<JobPostingSummaryDto>> GetPagedAsync(
        int pageNumber, int pageSize, string? location, string? employmentType, string? keyword)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(
            pageNumber, pageSize, location, employmentType, keyword);

        return new PagedResponse<JobPostingSummaryDto>
        {
            Items = _mapper.Map<List<JobPostingSummaryDto>>(items),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<JobPostingDto> CreateAsync(int recruiterId, CreateJobPostingDto request)
    {
        var jobPosting = _mapper.Map<JobPosting>(request);
        jobPosting.RecruiterId = recruiterId;

        await _repository.AddAsync(jobPosting);

        return _mapper.Map<JobPostingDto>(jobPosting);
    }

    public async Task<bool> UpdateAsync(int recruiterId, int jobPostingId, UpdateJobPostingDto request)
    {
        var jobPosting = await _repository.GetByIdAsync(jobPostingId);

        if (jobPosting == null)
            return false;

        if (jobPosting.RecruiterId != recruiterId)
            return false;

        _mapper.Map(request, jobPosting);
        await _repository.UpdateAsync(jobPosting);

        return true;
    }
}