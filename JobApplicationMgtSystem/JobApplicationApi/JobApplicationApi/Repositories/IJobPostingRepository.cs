using JobApplicationApi.Models;

namespace JobApplicationApi.Repositories
{
    public interface IJobPostingRepository
    {
        Task<JobPosting?> GetByIdAsync(int id);
        Task<(List<JobPosting> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? location, string? employmentType, string? keyword);
        Task<List<JobPosting>> GetByRecruiterIdAsync(int recruiterId);
        Task AddAsync(JobPosting jobPosting);
        Task UpdateAsync(JobPosting jobPosting);
    }
}
