using JobApplicationApi.Dtos;

namespace JobApplicationApi.Services
{
    public interface ICandidateService
    {
        Task<CandidateDto?> GetByIdAsync(int id);
        Task<CandidateDto?> GetByUserIdAsync(string userId);
        Task<bool> UpdateAsync(string userId, UpdateCandidateDto request);
    }
}
