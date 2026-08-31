using JobApplicationApi.Dtos;

namespace JobApplicationApi.Services
{
    public interface IInterviewStageService
    {
        Task<List<InterviewStageDto>> GetByJobApplicationIdAsync(int jobApplicationId, string userId, bool isRecruiter);
        Task<InterviewStageDto?> GetByIdAsync(int interviewStageId);
        Task<InterviewStageDto?> CreateAsync(int recruiterId, int jobApplicationId, CreateInterviewStageDto request);
        Task<bool> UpdateAsync(int recruiterId, int interviewStageId, UpdateInterviewStageDto request);
        Task<bool> DeleteAsync(int recruiterId, int interviewStageId);
    }
}
