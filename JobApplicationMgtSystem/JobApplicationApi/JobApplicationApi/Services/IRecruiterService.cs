using JobApplicationApi.Dtos;

namespace JobApplicationApi.Services
{
    public interface IRecruiterService
    {
        Task<RecruiterDto?> GetByIdAsync(int id);
        Task<RecruiterDto?> GetByUserIdAsync(string userId);
        Task<bool> UpdateAsync(string userId, UpdateRecruiterDto request);
    }
}
