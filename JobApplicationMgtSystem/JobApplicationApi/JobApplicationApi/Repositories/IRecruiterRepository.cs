using JobApplicationApi.Models;

namespace JobApplicationApi.Repositories
{
    public interface IRecruiterRepository
    {
        Task<Recruiter?> GetByIdAsync(int id);
        Task<Recruiter?> GetByUserIdAsync(string userId);
        Task UpdateAsync(Recruiter recruiter);
        Task AddAsync(Recruiter recruiter);
    }
}
