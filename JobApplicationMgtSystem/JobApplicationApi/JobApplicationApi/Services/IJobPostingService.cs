using JobApplicationApi.Dtos;

namespace JobApplicationApi.Services
{
    public interface IJobPostingService
    {
        Task<JobPostingDto?> GetByIdAsync(int id);
        Task<PagedResponse<JobPostingSummaryDto>> GetPagedAsync(int pageNumber, int pageSize, string? location, string? employmentType, string? keyword);
        Task<JobPostingDto> CreateAsync(int recruiterId, CreateJobPostingDto request);
        Task<bool> UpdateAsync(int recruiterId, int jobPostingId, UpdateJobPostingDto request);
    }
}
