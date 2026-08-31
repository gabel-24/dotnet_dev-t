using JobApplicationApi.Models;

namespace JobApplicationApi.Repositories
{
    public interface ICandidateRepository
    {
        Task<Candidate?> GetByIdAsync(int id);
        Task<Candidate?> GetByUserIdAsync(string id);
        Task UpdateInfoAsync(Candidate candidate);
        Task AddAsync(Candidate candidate);
    }
}
