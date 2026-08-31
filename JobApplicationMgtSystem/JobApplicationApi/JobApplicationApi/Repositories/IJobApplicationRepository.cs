using JobApplicationApi.Models;

namespace JobApplicationApi.Repositories
{
    public interface IJobApplicationRepository
    {
        Task<JobApplication?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int candidateId, int jobPostingId);
        Task<(List<JobApplication> Items, int TotalCount)> GetByCandidateIdAsync(int candidateId, int pageNumber, int pageSize);
        Task<(List<JobApplication> Items, int TotalCount)> GetByJobPostingIdAsync(int jobPostingId, int pageNumber, int pageSize);
        Task AddAsync(JobApplication jobApplication);
        Task UpdateAsync(JobApplication jobApplication);
    }
}
