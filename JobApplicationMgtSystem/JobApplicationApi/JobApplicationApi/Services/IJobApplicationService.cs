using JobApplicationApi.Dtos;

namespace JobApplicationApi.Services
{
    public interface IJobApplicationService
    {
        Task<JobApplicationDto?> GetByIdAsync(int id);
        Task<PagedResponse<JobApplicationSummaryDto>> GetByCandidateAsync(int candidateId, int pageNumber, int pageSize);
        Task<PagedResponse<JobApplicationSummaryDto>> GetByJobPostingAsync(int recruiterId, int jobPostingId, int pageNumber, int pageSize);
        Task<JobApplicationDto> CreateAsync(int candidateId, CreateJobApplicationDto request);
        Task<bool> UpdateStatusAsync(int recruiterId, int jobApplicationId, UpdateJobApplicationDto request);
    }
}
